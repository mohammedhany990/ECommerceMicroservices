using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommand : IRequest<CartDto>
    {
        public AddItemToCartCommand(Guid userId, Guid productId, int quantity)
        {
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }

        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }

    }
}
