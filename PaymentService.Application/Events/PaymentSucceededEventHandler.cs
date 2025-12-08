using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Messaging;

namespace PaymentService.Application.Events
{
    public class PaymentSucceededEventHandler : INotificationHandler<PaymentSucceededEvent>
    {
        private readonly IRabbitMqPublisher<PaymentSucceededEventMessage> _publisher;
        private readonly ILogger<PaymentSucceededEventHandler> _logger;

        public PaymentSucceededEventHandler(
            IRabbitMqPublisher<PaymentSucceededEventMessage> publisher,
            ILogger<PaymentSucceededEventHandler> logger)
        {
            _publisher = publisher;
            _logger = logger;
        }

        public Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
        {
            var message = new PaymentSucceededEventMessage
            {
                OrderId = notification.OrderId,
                PaymentId = notification.PaymentId,
                Amount = notification.Amount,
                Status = "Paid"
            };

            _publisher.Publish(message);

            _logger.LogInformation("Published PaymentSucceededEvent for Order {OrderId}", notification.OrderId);

            return Task.CompletedTask;
        }
    }
}
