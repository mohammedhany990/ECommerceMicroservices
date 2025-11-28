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
        public GetOrderByUserIdQuery(Guid userId, string authToken)
        {
            UserId = userId;
            AuthToken = authToken;
        }

        public Guid UserId { get; set; }
        public string AuthToken { get; set; }

        
    }
}
