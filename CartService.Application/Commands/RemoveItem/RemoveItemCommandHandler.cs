using CartService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Commands.RemoveItem
{
    public class RemoveItemCommandHandler : IRequestHandler<RemoveItemCommand, bool>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ILogger<RemoveItemCommandHandler> _logger;

        public RemoveItemCommandHandler(ICartRepository cartRepository, ILogger<RemoveItemCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _logger = logger;
        }

        public async Task<bool> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Removing item from cart. UserId: {UserId}, ProductId: {ProductId}", request.UserId, request.ProductId);

            var cart = await _cartRepository.GetCartAsync(request.UserId);
            if (cart is null || cart.Items is null || !cart.Items.Any())
            {
                _logger.LogWarning("Cart is empty or not found. UserId: {UserId}", request.UserId);
                return false;
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item == null)
            {
                _logger.LogWarning("Item not found in cart. ProductId: {ProductId}, UserId: {UserId}", request.ProductId, request.UserId);
                return false;
            }

            cart.Items.Remove(item);
            _logger.LogInformation("Item removed. ProductId: {ProductId}, UserId: {UserId}", request.ProductId, request.UserId);

            if (!cart.Items.Any())
            {
                await _cartRepository.DeleteCartAsync(request.UserId);
                _logger.LogInformation("Cart deleted because it is empty. UserId: {UserId}", request.UserId);
            }
            else
            {
                await _cartRepository.UpdateCartAsync(cart);
                _logger.LogInformation("Cart updated after item removal. UserId: {UserId}", request.UserId);
            }

            return true;
        }
    }
}
