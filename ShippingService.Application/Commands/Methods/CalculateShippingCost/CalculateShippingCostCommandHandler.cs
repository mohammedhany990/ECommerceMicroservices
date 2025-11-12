using MediatR;
using Shared.DTOs;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ShippingResultDto = Shared.DTOs.ShippingCostResultDto;
//using ShippingCostResultDto = Shared.DTOs.ShippingCostResultDto;
namespace ShippingService.Application.Commands.Methods.CalculateShippingCost
{
    public class CalculateShippingCostCommandHandler : IRequestHandler<CalculateShippingCostCommand, ShippingCostResultDto>
    {
        private readonly IRepository<ShippingMethod> _methodRepo;
        private readonly IRepository<ShippingAddress> _addressRepo;

        public CalculateShippingCostCommandHandler(
            IRepository<ShippingMethod> methodRepo,
            IRepository<ShippingAddress> addressRepo)
        {
            _methodRepo = methodRepo;
            _addressRepo = addressRepo;
        }

        public async Task<ShippingCostResultDto> Handle(CalculateShippingCostCommand request, CancellationToken cancellationToken)
        {
            var address = await _addressRepo.GetByIdAsync(request.ShippingAddressId);
            var method = await _methodRepo.GetByIdAsync(request.ShippingMethodId);

            if (address == null || method == null)
                throw new Exception("Invalid address or shipping method.");

            var cost = method.Cost;

            if (address.Country != "DefaultCountry")
                cost += 5;

            return new ShippingCostResultDto
            {
                Cost = cost,
                MethodName = method.Name,
                EstimatedDeliveryDays = method.EstimatedDeliveryDays
            };
        }
    }

}
