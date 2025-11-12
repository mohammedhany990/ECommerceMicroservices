using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.InfraStructure.Services;
using MediatR;
using Shared.DTOs;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using CartDto = CartService.Application.DTOs.CartDto;

namespace CartService.Application.Queries.GetCart
{
    public class GetCartQueryHandler : IRequestHandler<GetCartQuery, CartDto>
    {
        private readonly ICartRepository _cartRepository;
        private readonly IMapper _mapper;
        private readonly ShippingServiceClient _shippingServiceClient;

        public GetCartQueryHandler(
            ICartRepository cartRepository,
            IMapper mapper,
            ShippingServiceClient shippingServiceClient)
        {
            _cartRepository = cartRepository;
            _mapper = mapper;
            _shippingServiceClient = shippingServiceClient;
        }

        public async Task<CartDto> Handle(GetCartQuery request, CancellationToken cancellationToken)
        {
            
            var cart = await _cartRepository.GetCartAsync(request.UserId)
                       ?? new Cart { UserId = request.UserId, Items = new List<CartItem>() };

            var cartDto = _mapper.Map<CartDto>(cart);

            if (request.ShippingAddressId.HasValue && request.ShippingMethodId.HasValue)
            {
                var shippingResult = await _shippingServiceClient.CalculateShippingCostAsync(
                    new ShippingCostRequestDto(request.ShippingAddressId.Value, request.ShippingMethodId.Value));
                    

                cart.ShippingCost = shippingResult.Cost;

                cartDto.ShippingCost = cart.ShippingCost;
                cartDto.EstimatedDeliveryDays = shippingResult.EstimatedDeliveryDays;
                cartDto.Subtotal = cart.Subtotal;
                cartDto.TotalPrice = cartDto.Subtotal + cartDto.ShippingCost;

            }

            return cartDto;
        }
    }
}
