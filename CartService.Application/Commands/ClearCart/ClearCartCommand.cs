using MediatR;

namespace CartService.Application.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<bool>
    {
        public Guid UserId { get; }
        public ClearCartCommand(Guid userId) => UserId = userId;
    }

}
