using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddressById
{
    public class GetShippingAddressByIdQuery : IRequest<ShippingAddressDto>
    {
        public Guid ID { get; set; }

    }
}
