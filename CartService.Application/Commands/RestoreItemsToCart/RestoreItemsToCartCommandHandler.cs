using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using MediatR;
using Microsoft.Extensions.Logging;

namespace CartService.Application.Commands.RestoreItemsToCart
{
    public class RestoreItemsToCartCommandHandler : IRequestHandler<RestoreItemsToCartCommand, CartDto>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<RestoreItemsToCartCommandHandler> _logger;

        public RestoreItemsToCartCommandHandler(ICartRepository repository, IMapper mapper, ILogger<RestoreItemsToCartCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<CartDto> Handle(RestoreItemsToCartCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Restoring {ItemCount} items to cart for UserId: {UserId}", request.Items.Count, request.UserId);

            var cart = await _repository.GetCartAsync(request.UserId)
                       ?? new Cart { UserId = request.UserId, Items = new List<CartItem>() };

            foreach (var itemDto in request.Items)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = itemDto.Quantity;
                    _logger.LogInformation("Updated quantity for ProductId: {ProductId} to {Quantity}", itemDto.ProductId, itemDto.Quantity);
                }
                else
                {
                    var item = _mapper.Map<CartItem>(itemDto);
                    cart.AddItem(item);
                    _logger.LogInformation("Added item to cart. ProductId: {ProductId}, Quantity: {Quantity}", itemDto.ProductId, itemDto.Quantity);
                }
            }

            var updatedCart = await _repository.UpdateCartAsync(cart);
            _logger.LogInformation("Cart restored for UserId: {UserId}", request.UserId);

            return _mapper.Map<CartDto>(updatedCart);
        }
    }
}
