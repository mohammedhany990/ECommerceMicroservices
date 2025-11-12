using MediatR;
using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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
