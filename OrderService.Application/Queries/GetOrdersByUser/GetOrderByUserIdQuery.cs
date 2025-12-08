using MediatR;
using OrderService.Application.DTOs;

namespace OrderService.Application.Queries.GetOrdersByUser
{
    public class GetOrderByUserIdQuery : IRequest<IReadOnlyList<OrderDto>>
    {
        public GetOrderByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }

        public Guid UserId { get; set; }


    }
}
