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
        public string ProductId { get; set; } = string.Empty;
        public RemoveItemCommand(Guid userId, string productId)
        {
            UserId = userId;
            ProductId = productId;
        }

    }
}
