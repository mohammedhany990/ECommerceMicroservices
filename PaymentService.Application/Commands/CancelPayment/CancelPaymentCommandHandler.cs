using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
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

        public CancelPaymentCommandHandler(
            IPaymentRepository repository,
            IMapper mapper,
            IConfiguration configuration,
            IRabbitMqPublisher<CreateNotificationEvent> rabbitMqPublisher,
            UserServiceRpcClient userServiceRpcClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _configuration = configuration;
            _rabbitMqPublisher = rabbitMqPublisher;
            _userServiceRpcClient = userServiceRpcClient;
        }

        public async Task<PaymentResultDto> Handle(CancelPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _repository.GetByPaymentIntentIdAsync(request.PaymentIntentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            if (string.IsNullOrWhiteSpace(payment.PaymentIntentId))
                throw new Exception("This payment does not contain a valid PaymentIntentId.");

            if (payment.Status == PaymentStatus.Succeeded)
                throw new Exception("Cannot cancel a succeeded payment.");
            if (payment.Status == PaymentStatus.Failed)
                throw new Exception("Cannot cancel a failed payment.");

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var service = new PaymentIntentService();

            try
            {
                await service.CancelAsync(payment.PaymentIntentId, cancellationToken: cancellationToken);
            }
            catch (StripeException ex)
            {
                throw new Exception($"Stripe cancellation failed: {ex.Message}");
            }

            payment.Status = PaymentStatus.Canceled;
            payment.FailureReason = "Payment canceled by user.";
            payment.CanceledAt = DateTime.UtcNow;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();


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
            _ = Task.Run(() => _rabbitMqPublisher.Publish(notificationEvent));


            return _mapper.Map<PaymentResultDto>(payment);
        }
    }

}
