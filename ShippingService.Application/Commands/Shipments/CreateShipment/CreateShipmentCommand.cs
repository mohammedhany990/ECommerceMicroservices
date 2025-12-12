using MediatR;
using Shared.Enums;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Commands.Shipments.CreateShipment
{
    public class CreateShipmentCommand : IRequest<ShipmentDto>
    {
        public Guid OrderId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
        public string? TrackingNumber { get; set; }
        public ShipmentStatus Status { get; set; } = ShipmentStatus.Pending;
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }
    }
}
