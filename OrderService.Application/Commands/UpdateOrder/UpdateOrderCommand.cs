using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using Shared.Enums;

namespace OrderService.Application.Commands.UpdateOrder
{
    public class UpdateOrderCommand : IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public Guid? ShippingMethodId { get; set; }
        public DateTime? ExpectedDeliveryDate { get; set; }
        public OrderStatus? Status { get; set; }
    }

}
