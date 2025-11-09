using CartService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommand : IRequest<CartDto>
    {
        public Guid UserId { get; set; }
        public CartItemDto Item { get; set; }
        public AddItemToCartCommand(Guid userId, CartItemDto item)
        {
            UserId = userId;
            Item = item;
        }
    }
}
