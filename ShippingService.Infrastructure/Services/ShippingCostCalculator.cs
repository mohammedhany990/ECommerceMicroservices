using Shared.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Infrastructure.Services
{
    public class ShippingCostCalculator : IShippingCostCalculator
    {
        private readonly IRepository<ShippingAddress> _addressRepo;
        private readonly IRepository<ShippingMethod> _methodRepo;

        public ShippingCostCalculator(IRepository<ShippingAddress> addressRepo, IRepository<ShippingMethod> methodRepo)
        {
            _addressRepo = addressRepo;
            _methodRepo = methodRepo;
        }

        public async Task<ShippingCostResultDto> CalculateAsync(Guid addressId, Guid methodId)
        {
            var address = await _addressRepo.GetByIdAsync(addressId);
            var method = await _methodRepo.GetByIdAsync(methodId);

            if (address == null || method == null)
                throw new Exception("Invalid address or shipping method.");

            var cost = method.Cost;
            if (address.Country != "DefaultCountry")
                cost += 5;

            return new ShippingCostResultDto
            {
                Cost = cost,
                MethodName = method.Name,
                EstimatedDeliveryDays = method.EstimatedDeliveryDays,
                ShippingMethodId = methodId
            };
        }
    }
}
