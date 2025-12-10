using AutoMapper;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using CartDto = CartService.Application.DTOs.CartDto;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, CartDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ProductServiceRpcClient _productServiceRpcClient;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateItemQuantityCommandHandler> _logger;

        public UpdateItemQuantityCommandHandler(
            ICartRepository cartRepository,
            ProductServiceRpcClient productServiceRpcClient,
            IMapper mapper,
            ILogger<UpdateItemQuantityCommandHandler> logger)
        {
            _cartRepository = cartRepository;
            _productServiceRpcClient = productServiceRpcClient;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartDto> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Updating item quantity. UserId: {UserId}, ProductId: {ProductId}, Change: {Quantity}", request.UserId, request.ProductId, request.Quantity);

            var cart = await _cartRepository.GetCartAsync(request.UserId);
            if (cart == null || cart.Items == null || !cart.Items.Any())
            {
                _logger.LogWarning("Cart is empty or not found for UserId: {UserId}", request.UserId);
                return null;
            }

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item == null)
            {
                _logger.LogWarning("Item not found in cart. ProductId: {ProductId}, UserId: {UserId}", request.ProductId, request.UserId);
                return null;
            }

            var product = await _productServiceRpcClient.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found in product service. ProductId: {ProductId}", request.ProductId);
                throw new Exception("Product not found.");
            }

            var newQuantity = item.Quantity + request.Quantity;

            if (newQuantity <= 0)
            {
                cart.Items.Remove(item);
                _logger.LogInformation("Item removed from cart. ProductId: {ProductId}, UserId: {UserId}", request.ProductId, request.UserId);
            }
            else if (newQuantity > product.QuantityInStock)
            {
                _logger.LogWarning("Insufficient stock for ProductId: {ProductId}. Requested: {Quantity}, InStock: {Stock}", request.ProductId, newQuantity, product.QuantityInStock);
                throw new Exception("Insufficient stock.");
            }
            else
            {
                item.Quantity = newQuantity;
                _logger.LogInformation("Item quantity updated. ProductId: {ProductId}, NewQuantity: {Quantity}, UserId: {UserId}", request.ProductId, newQuantity, request.UserId);
            }

            cart.LastUpdated = DateTime.UtcNow;
            await _cartRepository.UpdateCartAsync(cart);

            var cartDto = _mapper.Map<CartDto>(cart);
            _logger.LogInformation("Cart updated successfully for UserId: {UserId}", request.UserId);

            return cartDto;
        }
    }
}
