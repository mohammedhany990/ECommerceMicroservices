using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, CartDto>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;
        private readonly ProductServiceRpcClient _productServiceRpcClient;

        public AddItemToCartCommandHandler(
            ICartRepository repository,
            IMapper mapper,
            ProductServiceRpcClient productServiceRpcClient
            )
        {
            _repository = repository;
            _mapper = mapper;
            _productServiceRpcClient = productServiceRpcClient;
        }

        public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            var product = await _productServiceRpcClient.GetProductByIdAsync(request.ProductId);
            if (product == null)
                throw new Exception("Product not found");
            if (product.QuantityInStock <= 0)
                throw new Exception("Product is out of stock");

            var cart = await _repository.GetCartAsync(request.UserId) ?? new Cart { UserId = request.UserId };

            var item = new CartItem
            {
                ProductId = product.Id,
                ProductName = product.Name,
                UnitPrice = product.Price,
                Quantity = request.Quantity,
                ImageUrl = product.ImageUrl
            };

            cart.AddItem(item);

            var updatedCart = await _repository.UpdateCartAsync(cart);

            var cartDto = _mapper.Map<CartDto>(updatedCart);
            return cartDto;
        }
    }
}
