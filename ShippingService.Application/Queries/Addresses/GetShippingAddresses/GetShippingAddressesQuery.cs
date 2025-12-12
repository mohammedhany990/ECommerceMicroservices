using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddresses
{
    public class GetShippingAddressesQuery : IRequest<List<ShippingAddressDto>>
    {
        public Guid UserId { get; set; }
        public GetShippingAddressesQuery(Guid userId)
        {
            UserId = userId;
        }

    }
}
