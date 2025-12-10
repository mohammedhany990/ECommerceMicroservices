using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, CartDto>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ProductServiceRpcClient _productServiceRpcClient;
        private readonly ILogger<AddItemToCartCommandHandler> _logger;

        public AddItemToCartCommandHandler(
            ICartRepository repository,
            IMapper mapper,
            ProductServiceRpcClient productServiceRpcClient,
            ILogger<AddItemToCartCommandHandler> logger) 
        {
            _repository = repository;
            _mapper = mapper;
            _productServiceRpcClient = productServiceRpcClient;
            _logger = logger;
        }

        public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Adding item to cart. UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}",
                request.UserId, request.ProductId, request.Quantity);

            var product = await _productServiceRpcClient.GetProductByIdAsync(request.ProductId);
            if (product == null)
            {
                _logger.LogWarning("Product not found. ProductId: {ProductId}", request.ProductId);
                throw new Exception("Product not found");
            }

            if (product.QuantityInStock <= 0)
            {
                _logger.LogWarning("Product out of stock. ProductId: {ProductId}", request.ProductId);
                throw new Exception("Product is out of stock");
            }

            var cart = await _repository.GetCartAsync(request.UserId) ?? new Cart { UserId = request.UserId };
            _logger.LogInformation("Cart retrieved for user {UserId}", request.UserId);

            var item = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = request.Quantity,
                ImageUrl = product.ImageUrl
            };

            cart.AddItem(item);
            _logger.LogInformation("Item added to cart. ProductId: {ProductId}", product.Id);

            var updatedCart = await _repository.UpdateCartAsync(cart);
            _logger.LogInformation("Cart updated for user {UserId}", request.UserId);

            var cartDto = _mapper.Map<CartDto>(updatedCart);
            return cartDto;
        }
    }
}
