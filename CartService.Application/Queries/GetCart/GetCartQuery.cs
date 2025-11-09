using CartService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Queries.GetCart
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public Guid UserId { get; set; }

        public GetCartQuery(Guid userId)
        {
            UserId = userId;
        }
    }
}
