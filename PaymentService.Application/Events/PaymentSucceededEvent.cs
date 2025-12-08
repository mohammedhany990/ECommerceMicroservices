using MediatR;

namespace PaymentService.Application.Events
{
    public class PaymentSucceededEvent : INotification
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
    }

}
