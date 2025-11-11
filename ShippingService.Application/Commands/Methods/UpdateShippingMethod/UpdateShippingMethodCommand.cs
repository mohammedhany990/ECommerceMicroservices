using MediatR;
using ShippingService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Methods.UpdateShippingMethod
{
    public class UpdateShippingMethodCommand : IRequest<ShippingMethodDto>
    {
        public Guid Id { get; set; }
        public string Name { get; set; }
        public decimal Cost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
        public bool IsActive { get; set; }
    }
}
