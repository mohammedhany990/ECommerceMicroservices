using MediatR;
using Shared.DTOs;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
//using ShippingCostResultDto = Shared.DTOs.ShippingCostResultDto;

namespace ShippingService.Application.Commands.Methods.CalculateShippingCost
{
    public class CalculateShippingCostCommand : IRequest<ShippingCostResultDto>
    {
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
    }
}
