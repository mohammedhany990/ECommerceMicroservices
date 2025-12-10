using CartService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<ClearCartCommandHandler> _logger;

        public ClearCartCommandHandler(ICartRepository cartRepository, ILogger<ClearCartCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Clearing cart for UserId: {UserId}", request.UserId);

            var result = await _cartRepository.ClearCartAsync(request.UserId);

            if (result)
                _logger.LogInformation("Cart cleared successfully for UserId: {UserId}", request.UserId);
            else
                _logger.LogWarning("Failed to clear cart or cart was already empty. UserId: {UserId}", request.UserId);

            return result;
        }
    }
}
