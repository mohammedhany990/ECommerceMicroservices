using CartService.Domain.Interfaces;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.ClearCart
{
    public class ClearCartCommandHandler : IRequestHandler<ClearCartCommand, bool>
    {
        private readonly ICartRepository _cartRepository;

        public ClearCartCommandHandler(ICartRepository cartRepository)
        {
            _cartRepository = cartRepository;
        }
        public async Task<bool> Handle(ClearCartCommand request, CancellationToken cancellationToken)
        {
            var result = await _cartRepository.ClearCartAsync(request.UserId);
            return result;
        }
    }
}
