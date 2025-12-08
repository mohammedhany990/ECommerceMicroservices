using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrderById
{
    public class GetOrderByIdQuery : IRequest<OrderDto>
    {
        public Guid OrderId { get; }
        public GetOrderByIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }

    }
}
