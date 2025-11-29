using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Infrastructure.Services
{
    public interface IShippingCostCalculator
    {
        Task<ShippingCostResultDto> CalculateAsync(Guid addressId, Guid methodId);
    }

}
