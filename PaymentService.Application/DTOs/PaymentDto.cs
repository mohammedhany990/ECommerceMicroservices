namespace PaymentService.Application.DTOs
{
    public class PaymentDto
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";

        public string Status { get; set; } = string.Empty;

        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? RefundedAt { get; set; }
    }

}
