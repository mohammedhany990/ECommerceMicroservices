using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using MediatR;

namespace CartService.Application.Commands.RestoreItemsToCart
{
    public class RestoreItemsToCartCommandHandler : IRequestHandler<RestoreItemsToCartCommand, CartDto>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public RestoreItemsToCartCommandHandler(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<CartDto> Handle(RestoreItemsToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetCartAsync(request.UserId)
                       ?? new Cart { UserId = request.UserId, Items = new List<CartItem>() };

            foreach (var itemDto in request.Items)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == itemDto.ProductId);
                if (existingItem != null)
                {
                    existingItem.Quantity = itemDto.Quantity;
                }
                else
                {
                    var item = _mapper.Map<CartItem>(itemDto);
                    cart.AddItem(item);
                }
            }


            var updatedCart = await _repository.UpdateCartAsync(cart);
            return _mapper.Map<CartDto>(updatedCart);
        }


    }
}
