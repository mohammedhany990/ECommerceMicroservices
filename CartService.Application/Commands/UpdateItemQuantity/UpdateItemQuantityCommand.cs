using CartService.Application.DTOs;
using MediatR;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommand : IRequest<CartDto>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public int Quantity { get; set; }
        public UpdateItemQuantityCommand(Guid userId, Guid productId, int quantity)
        {
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
