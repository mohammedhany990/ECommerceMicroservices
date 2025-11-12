using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.RemoveItem
{
    public class RemoveItemCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public Guid ProductId { get; set; }
        public RemoveItemCommand(Guid userId, Guid productId)
        {
            UserId = userId;
            ProductId = productId;
        }

    }
}
