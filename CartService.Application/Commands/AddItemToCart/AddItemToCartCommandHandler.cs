using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

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
            if (request.Quantity <= 0)
            {
                _logger.LogWarning("Invalid quantity requested. Quantity must be greater than 0. UserId: {UserId}, ProductId: {ProductId}", request.UserId, request.ProductId);
                throw new ArgumentException("Quantity must be greater than 0");
            }

            _logger.LogInformation("Adding item to cart. UserId: {UserId}, ProductId: {ProductId}, Quantity: {Quantity}", request.UserId, request.ProductId, request.Quantity);

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

            var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == product.Id);
            if (existingItem is not null)
            {
                var newQuantity = existingItem.Quantity + request.Quantity;

                if (newQuantity > product.QuantityInStock)
                {
                    _logger.LogWarning(
                        "Requested quantity exceeds stock. Requested total: {RequestedTotal}, Available: {Available}, ProductId: {ProductId}",
                        newQuantity, product.QuantityInStock, product.Id);
                    throw new Exception($"Cannot add {request.Quantity} items. Only {product.QuantityInStock - existingItem.Quantity} left in stock.");
                }

                existingItem.Quantity = newQuantity;
                _logger.LogInformation("Existing cart item quantity updated. ProductId: {ProductId}, NewQuantity: {Quantity}", product.Id, existingItem.Quantity);
            }
            else
            {
                if (request.Quantity > product.QuantityInStock)
                {
                    _logger.LogWarning(
                        "Requested quantity exceeds stock. Requested: {Requested}, Available: {Available}, ProductId: {ProductId}",
                        request.Quantity, product.QuantityInStock, product.Id);
                    throw new Exception($"Requested quantity ({request.Quantity}) exceeds available stock ({product.QuantityInStock})");
                }

                var newItem = new CartItem
                {
                    ProductId = product.Id,
                    ProductName = product.Name,
                    UnitPrice = product.Price,
                    Quantity = request.Quantity,
                    ImageUrl = product.ImageUrl
                };

                cart.AddItem(newItem);
                _logger.LogInformation("New item added to cart. ProductId: {ProductId}, Quantity: {Quantity}", product.Id, newItem.Quantity);
            }

            var updatedCart = await _repository.UpdateCartAsync(cart);
            _logger.LogInformation("Cart updated for user {UserId}", request.UserId);

            var cartDto = _mapper.Map<CartDto>(updatedCart);
            return cartDto;
        }
    }
}
