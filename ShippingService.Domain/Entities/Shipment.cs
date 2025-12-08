namespace ShippingService.Domain.Entities
{
    public class Shipment
    {
        public Guid Id { get; set; }
        public Guid OrderId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }

        public string TrackingNumber { get; set; } = "TBD";
        public string Status { get; set; } = "Pending";

        public DateTime ShippedAt { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }
    }
}
