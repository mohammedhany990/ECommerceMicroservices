namespace Shared.Messaging
{
    public class PaymentSucceededEventMessage
    {
        public Guid OrderId { get; set; }
        public Guid PaymentId { get; set; }
        public decimal Amount { get; set; }
        public string Status { get; set; }
    }

}
