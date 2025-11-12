using MediatR;
using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Queries.GetOrdersByUser
{
    public class GetOrderByUserIdQuery : IRequest<IReadOnlyList<OrderDto>>
    {
        public Guid UserId { get; set; }
        public GetOrderByUserIdQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
