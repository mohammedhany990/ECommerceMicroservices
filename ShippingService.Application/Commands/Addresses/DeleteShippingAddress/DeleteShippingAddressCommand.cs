using MediatR;

namespace ShippingService.Application.Commands.Addresses.DeleteShippingAddress
{
    public class DeleteShippingAddressCommand : IRequest<bool>
    {
        public Guid Id { get; set; }

    }
}
