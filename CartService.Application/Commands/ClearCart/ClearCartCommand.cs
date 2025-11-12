using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.ClearCart
{
    public class ClearCartCommand : IRequest<bool>
    {
        public Guid UserId { get; }
        public ClearCartCommand(Guid userId) => UserId = userId;
    }

}
