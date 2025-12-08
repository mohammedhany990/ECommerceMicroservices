using MediatR;
using ShippingService.Application.DTOs;

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
