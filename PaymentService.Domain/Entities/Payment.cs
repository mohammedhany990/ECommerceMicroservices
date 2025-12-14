using Shared.Enums;
 

namespace PaymentService.Domain.Entities
{
    public class Payment
    {
        public Guid Id { get; set; }

        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }

        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";

        public PaymentStatus Status { get; set; } = PaymentStatus.Pending;

        public string PaymentIntentId { get; set; } = string.Empty;
        public string ClientSecret { get; set; } = string.Empty;

        public string? FailureReason { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public DateTime? CanceledAt { get; set; }
        public DateTime? RefundedAt { get; set; }
    }
}
