using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddresses
{
    public class GetShippingAddressesQuery : IRequest<List<ShippingAddressDto>>
    {
    }
}
