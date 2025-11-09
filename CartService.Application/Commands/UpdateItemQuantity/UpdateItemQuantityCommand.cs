using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Commands.UpdateItemQuantity
{
    public class UpdateItemQuantityCommand : IRequest<bool>
    {
        public Guid UserId { get; set; }
        public string ProductId { get; set; }
        public int Quantity { get; set; }
        public UpdateItemQuantityCommand(Guid userId, string productId, int quantity)
        {
            UserId = userId;
            ProductId = productId;
            Quantity = quantity;
        }
    }
}
