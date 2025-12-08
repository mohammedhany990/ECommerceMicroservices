using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;

namespace OrderService.Application.Commands.UpdateOrderStatus
{
    public class UpdateOrderStatusCommand : IRequest<OrderDto>
    {
        public UpdateOrderStatusCommand(Guid orderId, OrderStatus newStatus)
        {
            OrderId = orderId;
            NewStatus = newStatus;
        }

        public Guid OrderId { get; set; }
        public OrderStatus NewStatus { get; set; }
    }
}
