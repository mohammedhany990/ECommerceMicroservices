using MediatR;
using Shared.DTOs;
//using ShippingCostResultDto = Shared.DTOs.ShippingCostResultDto;

namespace ShippingService.Application.Commands.Methods.CalculateShippingCost
{
    public class CalculateShippingCostCommand : IRequest<ShippingCostResultDto>
    {
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
    }
}
