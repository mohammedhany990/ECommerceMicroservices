using MediatR;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Methods.CreateShippingMethod
{
    public class CreateShippingMethodCommand : IRequest<ShippingMethodDto>
    {
        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public bool IsActive { get; set; } = true;
    }
}
