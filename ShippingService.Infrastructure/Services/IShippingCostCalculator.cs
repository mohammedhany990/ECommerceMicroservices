using Shared.DTOs;

namespace ShippingService.Infrastructure.Services
{
    public interface IShippingCostCalculator
    {
        Task<ShippingCostResultDto> CalculateAsync(Guid addressId, Guid methodId);
    }

}
