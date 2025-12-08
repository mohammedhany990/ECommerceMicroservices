using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Methods.GetShippingMethodById
{
    public class GetShippingMethodByIdQuery : IRequest<ShippingMethodDto>
    {
        public Guid Id { get; set; }

    }
}
