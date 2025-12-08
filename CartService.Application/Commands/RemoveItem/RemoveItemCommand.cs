using MediatR;

namespace CartService.Application.Commands.RemoveItem
{
    public class RemoveItemCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public RemoveItemCommand(Guid userId, Guid productId)
        {
            UserId = userId;
            ProductId = productId;
        }

    }
}
