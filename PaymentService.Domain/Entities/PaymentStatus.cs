namespace PaymentService.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Processing,
        Failed,
        Refunded,
        Canceled,
        Confirmed
    }

}
