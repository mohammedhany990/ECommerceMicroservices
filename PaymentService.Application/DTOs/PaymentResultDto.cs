namespace PaymentService.Application.DTOs
{
    public class PaymentResultDto
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string Status { get; set; } = string.Empty;
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public string? FailureReason { get; set; }
        public string? PaymentIntentId { get; set; }

    }
}
