using CartService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommandHandler : IRequestHandler<UpdateItemQuantityCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public UpdateItemQuantityCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(UpdateItemQuantityCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(request.UserId.ToString());
            if (cart is null || cart.Items is null || !cart.Items.Any())
                return false;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);
            if (item is null)
                return false;

            item.Quantity += request.Quantity;

            if (item.Quantity <= 0)
                cart.Items.Remove(item);

            await _cartRepository.UpdateCartAsync(cart);

            return true;
        }
    }
}
