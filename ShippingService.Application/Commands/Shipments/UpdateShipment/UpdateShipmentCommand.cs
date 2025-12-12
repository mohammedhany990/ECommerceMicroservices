using MediatR;
using Shared.Enums;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Commands.Shipments.UpdateShipment
{
    public class UpdateShipmentCommand : IRequest<ShipmentDto>
    {
        public Guid Id { get; set; }

        public Guid? OrderId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public Guid? ShippingMethodId { get; set; }
        public string? TrackingNumber { get; set; }
        public string? Status { get; set; }
        public DateTime? ShippedAt { get; set; }
        public DateTime? DeliveredAt { get; set; }

    }
}
