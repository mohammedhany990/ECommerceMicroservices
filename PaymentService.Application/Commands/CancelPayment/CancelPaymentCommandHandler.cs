using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using Shared.Enums;
using Shared.Messaging;
using Stripe;

namespace PaymentService.Application.Commands.CancelPayment
{
    public class CancelPaymentCommandHandler : IRequestHandler<CancelPaymentCommand, PaymentResultDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _rabbitMqPublisher;
        private readonly UserServiceRpcClient _userServiceRpcClient;
        private readonly ILogger<CancelPaymentCommandHandler> _logger;

        public CancelPaymentCommandHandler(
            IPaymentRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IRabbitMqPublisher<CreateNotificationEvent> rabbitMqPublisher,
            UserServiceRpcClient userServiceRpcClient,
            ILogger<CancelPaymentCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _rabbitMqPublisher = rabbitMqPublisher;
            _userServiceRpcClient = userServiceRpcClient;
            _logger = logger;
        }

        public async Task<PaymentResultDto> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting payment cancellation for PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);

            var payment = await _repository.GetByPaymentIntentIdAsync(request.PaymentIntentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for PaymentIntentId: {PaymentIntentId}", request.PaymentIntentId);
                throw new Exception("Payment not found.");
            }

            if (string.IsNullOrWhiteSpace(payment.PaymentIntentId))
            {
                _logger.LogWarning("Payment does not contain a valid PaymentIntentId. PaymentId: {PaymentId}", payment.Id);
                throw new Exception("This payment does not contain a valid PaymentIntentId.");
            }

            if (payment.Status == PaymentStatus.Succeeded)
            {
                _logger.LogWarning("Cannot cancel a succeeded payment. PaymentId: {PaymentId}", payment.Id);
                throw new Exception("Cannot cancel a succeeded payment.");
            }

            if (payment.Status == PaymentStatus.Failed)
            {
                _logger.LogWarning("Cannot cancel a failed payment. PaymentId: {PaymentId}", payment.Id);
                throw new Exception("Cannot cancel a failed payment.");
            }

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new PaymentIntentService();

            try
            {
                _logger.LogInformation("Attempting to cancel payment on Stripe. PaymentIntentId: {PaymentIntentId}", payment.PaymentIntentId);
                await service.CancelAsync(payment.PaymentIntentId, cancellationToken: cancellationToken);
                _logger.LogInformation("Stripe payment canceled successfully. PaymentIntentId: {PaymentIntentId}", payment.PaymentIntentId);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe cancellation failed for PaymentIntentId: {PaymentIntentId}", payment.PaymentIntentId);
                throw new Exception($"Stripe cancellation failed: {ex.Message}");
            }

            payment.Status = PaymentStatus.Canceled;
            payment.FailureReason = "Payment canceled by user.";
            payment.CanceledAt = DateTime.UtcNow;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Payment updated in database as canceled. PaymentId: {PaymentId}", payment.Id);

            var userEmail = await _userServiceRpcClient.GetUserEmailAsync(payment.UserId);
            var notificationEvent = new CreateNotificationEvent
            {
                UserId = payment.UserId,
                To = userEmail,
                Subject = "Payment Canceled",
                Body = $@"
                        Hello,

                        We want to inform you that your payment with ID: {payment.Id} 
                        and amount: ${payment.Amount} has been canceled successfully.

                        Canceled At: {payment.CanceledAt?.ToString("yyyy-MM-dd HH:mm:ss")} UTC

                        If you did not request this cancellation, please contact support immediately.

                        Thank you,
                        Ecommerce
                        "
            };

            _ = Task.Run(() =>
            {
                _logger.LogInformation("Publishing payment cancellation notification to RabbitMQ for UserId: {UserId}", payment.UserId);
                _rabbitMqPublisher.Publish(notificationEvent);
            });

            _logger.LogInformation("Payment cancellation process completed for PaymentId: {PaymentId}", payment.Id);

            return _mapper.Map<PaymentResultDto>(payment);
        }
    }
}