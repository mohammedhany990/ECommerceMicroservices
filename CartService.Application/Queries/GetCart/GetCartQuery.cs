using CartService.Application.DTOs;
using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CartService.Application.Queries.GetCart
{
    public class GetCartQuery : IRequest<CartDto>
    {
        public GetCartQuery(Guid userId, Guid? shippingAddressId, Guid? shippingMethodId)
        {
            UserId = userId;
            ShippingAddressId = shippingAddressId;
            ShippingMethodId = shippingMethodId;
        }

        public Guid UserId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public Guid? ShippingMethodId { get; set; }
    }
}
