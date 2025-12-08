using MediatR;
using Shared.DTOs;
using ShippingService.Infrastructure.Services;
namespace ShippingService.Application.Commands.Methods.CalculateShippingCost
{
    public class CalculateShippingCostCommandHandler : IRequestHandler<CalculateShippingCostCommand, ShippingCostResultDto>
    {
        private readonly IShippingCostCalculator _calculator;

        public CalculateShippingCostCommandHandler(IShippingCostCalculator calculator)
        {
            _calculator = calculator;
        }

        public async Task<ShippingCostResultDto> Handle(CalculateShippingCostCommand request, CancellationToken cancellationToken)
        {
            return await _calculator.CalculateAsync(request.ShippingAddressId, request.ShippingMethodId);
        }
    }

}
