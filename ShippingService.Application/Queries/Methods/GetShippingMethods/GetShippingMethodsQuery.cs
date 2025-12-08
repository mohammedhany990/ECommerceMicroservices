using MediatR;
using ShippingService.Application.DTOs;

namespace ShippingService.Application.Queries.Methods.GetShippingMethods
{
    public class GetShippingMethodsQuery : IRequest<List<ShippingMethodDto>>
    {
    }
}
