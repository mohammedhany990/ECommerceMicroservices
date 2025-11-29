using AutoMapper;
using MediatR;
using Microsoft.Extensions.Configuration;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Services;
using Shared.Messaging;
using Stripe;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.RefundPayment
{

    public class RefundPaymentCommandHandler : IRequestHandler<RefundPaymentCommand, PaymentDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;
        private readonly IConfiguration _configuration;
        private readonly IRabbitMqPublisher<CreateNotificationEvent> _rabbitMqPublisher;
        private readonly UserServiceRpcClient _userServiceRpcClient;

        public RefundPaymentCommandHandler(
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

        public async Task<PaymentDto> Handle(RefundPaymentCommand request, CancellationToken cancellationToken)
        {
            var payment = await _repository.GetByIdAsync(request.PaymentId);
            if (payment == null)
                throw new Exception("Payment not found.");

            if (payment.Status != PaymentStatus.Succeeded)
                throw new Exception("Only succeeded payments can be refunded.");

            StripeConfiguration.ApiKey = _configuration["Stripe:SecretKey"];
            var refundService = new RefundService();

            var options = new RefundCreateOptions
            {
                PaymentIntent = payment.PaymentIntentId,
                Amount = (long)(payment.Amount * 100)
            };

            try
            {
                await refundService.CreateAsync(options, cancellationToken: cancellationToken);
            }
            catch (StripeException ex)
            {
                throw new Exception($"Stripe refund failed: {ex.Message}");
            }

            payment.Status = PaymentStatus.Refunded;
            payment.RefundedAt = DateTime.UtcNow;

            await _repository.UpdateAsync(payment);
            await _repository.SaveChangesAsync();

            var userEmail = await _userServiceRpcClient.GetUserEmailAsync(payment.UserId);

            var notificationEvent = new CreateNotificationEvent
            {
                UserId = payment.UserId,
                To = userEmail,
                Subject = "Payment Refunded",
                Body = $"Your payment for order {payment.OrderId} has been refunded.",
            };
            _rabbitMqPublisher.Publish(notificationEvent);



            return _mapper.Map<PaymentDto>(payment);
        }
    }



}
