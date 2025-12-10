using AutoMapper;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.Messaging;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.DTOs;
using CartDto = CartService.Application.DTOs.CartDto;

namespace CartService.Application.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ShippingServiceRpcClient _shippingServiceRpcClient;
        private readonly ILogger<GetCartQueryHandler> _logger;

        public GetCartQueryHandler(
            ICartRepository cartRepository,
            IMapper mapper,
            ShippingServiceRpcClient shippingServiceRpcClient,
            ILogger<GetCartQueryHandler> logger)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _shippingServiceRpcClient = shippingServiceRpcClient;
            _logger = logger;
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Retrieving cart for UserId: {UserId}", request.UserId);

            var cart = await _cartRepository.GetCartAsync(request.UserId)
                       ?? new Cart { UserId = request.UserId, Items = new List<CartItem>() };

            var cartDto = _mapper.Map<CartDto>(cart);

            if (request.ShippingAddressId.HasValue && request.ShippingMethodId.HasValue)
            {
                var shippingResult = await _shippingServiceRpcClient.CalculateShippingCostAsync(
                    new ShippingCostRequestDto(request.ShippingAddressId.Value, request.ShippingMethodId.Value));

                cart.ShippingCost = shippingResult.Cost;

                cartDto.ShippingCost = cart?.ShippingCost ?? 0;
                cartDto.EstimatedDeliveryDays = shippingResult.EstimatedDeliveryDays;
                cartDto.Subtotal = cart.Subtotal;
                cartDto.TotalPrice = cartDto.Subtotal + cartDto.ShippingCost;

                _logger.LogInformation("Calculated shipping cost: {ShippingCost}, EstimatedDeliveryDays: {Days} for UserId: {UserId}",
                    cartDto.ShippingCost, cartDto.EstimatedDeliveryDays, request.UserId);
            }

            return cartDto;
        }
    }
}
