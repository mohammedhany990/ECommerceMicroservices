using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Messaging;
using Shared.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.MarkPaidTest
{
    public class MarkPaidCommandHandler : IRequestHandler<MarkPaidCommand, bool>
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly OrderServiceRpcClient _orderServiceRpcClient;
        private readonly ILogger<MarkPaidCommandHandler> _logger;

        public MarkPaidCommandHandler(
            IPaymentRepository paymentRepository,
            OrderServiceRpcClient orderServiceRpcClient,
            ILogger<MarkPaidCommandHandler> logger)
        {
            _paymentRepository = paymentRepository;
            _orderServiceRpcClient = orderServiceRpcClient;
            _logger = logger;
        }

        public async Task<bool> Handle(MarkPaidCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation(
                "Manually marking payment as PAID for OrderId {OrderId}",
                request.OrderId);

            var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);
            if (payment == null)
            {
                _logger.LogWarning(
                    "Payment not found for OrderId {OrderId}",
                    request.OrderId);
                return false;
            }

            if (payment.Status == PaymentStatus.Paid)
            {
                _logger.LogInformation(
                    "Payment already PAID for OrderId {OrderId}",
                    request.OrderId);
                return true;
            }

            payment.Status = PaymentStatus.Paid;
            payment.ConfirmedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Payment status updated to PAID for PaymentId {PaymentId}",
                payment.Id);

            var orderUpdated = await _orderServiceRpcClient
                .UpdateOrderPaymentStatusAsync(request.OrderId, PaymentStatus.Paid);

            if (!orderUpdated)
            {
                _logger.LogWarning(
                    "Failed to update Order.PaymentStatus for OrderId {OrderId}",
                    request.OrderId);
            }

            return orderUpdated;
        }
    }
}
