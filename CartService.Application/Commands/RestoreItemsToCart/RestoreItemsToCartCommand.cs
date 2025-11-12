using CartService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.RestoreItemsToCart
{
    public class RestoreItemsToCartCommand : IRequest<CartDto>
    {
        public Guid UserId { get; }
        public List<CartItemDto> Items { get; }

        public RestoreItemsToCartCommand(Guid userId, List<CartItemDto> items)
        {
            UserId = userId;
            Items = items;
        }
    }


}
