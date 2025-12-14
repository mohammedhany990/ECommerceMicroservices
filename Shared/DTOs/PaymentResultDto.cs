using Shared.Enums;

namespace Shared.DTOs
{
    public class PaymentResultDto
    {
        public Guid PaymentId { get; set; }
        public Guid OrderId { get; set; }
        public decimal Amount { get; set; }
        public string Currency { get; set; } = "usd";
        public string Status { get; set; }
        public DateTime? ConfirmedAt { get; set; }
        public string? FailureReason { get; set; }
    }
}
