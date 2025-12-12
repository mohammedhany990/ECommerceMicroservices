using Shared.Enums;

namespace ShippingService.Application.DTOs
{
    public class ShipmentDto
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }

        public string TrackingNumber { get; set; } = "TBD";

        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

        public decimal ShippingCost { get; set; }

        public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }
}
