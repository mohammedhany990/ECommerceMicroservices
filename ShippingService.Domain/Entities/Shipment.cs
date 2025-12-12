using Shared.Enums;

namespace ShippingService.Domain.Entities
{
    public class Shipment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;

        public string TrackingNumber { get; set; } = "TBD";
        public decimal ShippingCost { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;


        public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
    }
}
