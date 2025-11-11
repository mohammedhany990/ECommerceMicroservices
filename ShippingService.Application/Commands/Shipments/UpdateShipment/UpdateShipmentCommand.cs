using MediatR;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
