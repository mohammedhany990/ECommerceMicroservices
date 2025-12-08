using MediatR;

namespace ShippingService.Application.Commands.Shipments.DeleteShipment
{
    public class DeleteShipmentCommand : IRequest<bool>
    {
        public Guid Id { get; set; }
    }
}
