using CartService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.RemoveItem
{
    public class RemoveItemCommandHandler : IRequestHandler<RemoveItemCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public RemoveItemCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }

        public async Task<bool> Handle(RemoveItemCommand request, CancellationToken cancellationToken)
        {
            var cart = await _cartRepository.GetCartAsync(request.UserId.ToString());

            if (cart is null || cart.Items is null || !cart.Items.Any())
                return false;

            var item = cart.Items.FirstOrDefault(i => i.ProductId == request.ProductId);

            if (item == null)
                return false;

            cart.Items.Remove(item);
    
            await _cartRepository.UpdateCartAsync(cart);

            return true;
        }
    }
}
