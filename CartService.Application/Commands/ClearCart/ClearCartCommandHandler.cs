using CartService.Domain.Interfaces;
using MediatR;

namespace CartService.Application.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public ClearCartCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var result = await _cartRepository.ClearCartAsync(request.UserId);
            return result;
        }
    }
}
