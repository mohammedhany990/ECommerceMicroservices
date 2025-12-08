using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Shipments.GetShipmentById
{
    public class GetShipmentByIdQuery : IRequest<ShipmentDto>
    {
        public Guid Id { get; set; }

    }
}
