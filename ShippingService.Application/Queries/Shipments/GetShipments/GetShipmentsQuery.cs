using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Shipments.GetShipments
{
    public class GetShipmentsQuery : IRequest<List<ShipmentDto>>
    {
    }
}
