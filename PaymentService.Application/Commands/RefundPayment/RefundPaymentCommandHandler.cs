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

namespace PaymentService.Application.Commands.RefundPayment
{
    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _rabbitMqPublisher;
        private readonly UserServiceRpcClient _userServiceRpcClient;
        private readonly ILogger<RefundPaymentCommandHandler> _logger;

        public RefundPaymentCommandHandler(
            IPaymentRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IRabbitMqPublisher<CreateNotificationEvent> rabbitMqPublisher,
            UserServiceRpcClient userServiceRpcClient,
            ILogger<RefundPaymentCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _rabbitMqPublisher = rabbitMqPublisher;
            _userServiceRpcClient = userServiceRpcClient;
            _logger = logger;
        }

        public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting refund for PaymentId: {PaymentId}", request.PaymentId);

            var payment = await _repository.GetByIdAsync(request.PaymentId);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found. PaymentId: {PaymentId}", request.PaymentId);
                throw new Exception("Payment not found.");
            }

            if (payment.Status != PaymentStatus.Succeeded)
            {
                _logger.LogWarning("Cannot refund payment that is not succeeded. PaymentId: {PaymentId}, Status: {Status}", payment.Id, payment.Status);
                throw new Exception("Only succeeded payments can be refunded.");
            }

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var refundService = new RefundService();

            var options = new RefundCreateOptions
            {
                PaymentIntent = payment.PaymentIntentId,
                Amount = (long)(payment.Amount * 100)
            };

            _logger.LogInformation("Creating Stripe refund for PaymentIntentId: {PaymentIntentId}, Amount: {Amount}", payment.PaymentIntentId, payment.Amount);
            try
            {
                await refundService.CreateAsync(options, cancellationToken: cancellationToken);
                _logger.LogInformation("Stripe refund created successfully for PaymentIntentId: {PaymentIntentId}", payment.PaymentIntentId);
            }
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Stripe refund failed for PaymentIntentId: {PaymentIntentId}", payment.PaymentIntentId);
                throw new Exception($"Stripe refund failed: {ex.Message}");
            }

            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();
            _logger.LogInformation("Payment updated in database as refunded. PaymentId: {PaymentId}", payment.Id);

            var userEmail = await _userServiceRpcClient.GetUserEmailAsync(payment.UserId);
            var notificationEvent = new CreateNotificationEvent
            {
                UserId = payment.UserId,
                To = userEmail,
                Subject = "Payment Refunded",
                Body = $"Your payment for order {payment.OrderId} has been refunded.",
            };

            _logger.LogInformation("Publishing refund notification to RabbitMQ for UserId: {UserId}", payment.UserId);
            _rabbitMqPublisher.Publish(notificationEvent);

            _logger.LogInformation("Refund process completed for PaymentId: {PaymentId}", payment.Id);

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
