using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;
using Shared.DTOs;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CartDto = CartService.Application.DTOs.CartDto;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, CartDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly ProductServiceRpcClient _productServiceRpcClient;
        private readonly IMapper _mapper;

        public UpdateItemQuantityCommandHandler(
            ICartRepository cartRepository,
            ProductServiceRpcClient productServiceRpcClient,
            IMapper mapper)
        {
            _cartRepository = cartRepository;
            _productServiceRpcClient = productServiceRpcClient;
            _mapper = mapper;
        }

        public async Task<CartDto> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(request.UserId);
            if (cart == null)
                return null;

            if (cart.Items == null || !cart.Items.Any())
                return null;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item == null)
                return null;

            var product = await _productServiceRpcClient.GetProductByIdAsync(request.ProductId);
            if (product == null)
                throw new Exception("Product not found.");

            var newQuantity = item.Quantity + request.Quantity;

            if (newQuantity <= 0)
            {
                cart.Items.Remove(item);
            }
            else if (newQuantity > product.QuantityInStock)
            {
                throw new Exception("Insufficient stock.");
            }
            else
            {
                item.Quantity = newQuantity;
            }

            cart.LastUpdated = DateTime.UtcNow;
            await _cartRepository.UpdateCartAsync(cart);

            var cartDto = _mapper.Map<CartDto>(cart);

            return cartDto;
        }
    }
}
