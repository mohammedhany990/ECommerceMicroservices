using AutoMapper;
using CartService.Application.DTOs;
using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using CartService.InfraStructure.Repositories;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.AddItemToCart
{
    public class AddItemToCartCommandHandler : IRequestHandler<AddItemToCartCommand, CartDto>
    {
        private readonly ICartRepository _repository;
        private readonly IMapper _mapper;

        public AddItemToCartCommandHandler(ICartRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<CartDto> Handle(AddItemToCartCommand request, CancellationToken cancellationToken)
        {
            var cart = await _repository.GetCartAsync(request.UserId.ToString()) ?? new Cart { UserId = request.UserId.ToString() };

            var item = _mapper.Map<CartItem>(request.Item);

            cart.AddItem(item);

            var updatedCart = await _repository.UpdateCartAsync(cart);

            return _mapper.Map<CartDto>(updatedCart);
        }
    }
}
