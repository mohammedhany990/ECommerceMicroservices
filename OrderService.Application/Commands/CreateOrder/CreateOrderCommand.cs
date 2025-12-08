using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public Guid UserId { get; set; }
        public string Email { get; set; }

        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }

    }

}
