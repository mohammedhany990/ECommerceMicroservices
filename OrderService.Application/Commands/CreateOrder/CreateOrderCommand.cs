using MediatR;
using OrderService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.CreateOrder
{
    public class CreateOrderCommand : IRequest<OrderDto>
    {
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }

        public Guid ShippingMethodId { get; set; }

        public string? PaymentMethodId { get; set; }
        public string AuthToken { get; set; } = string.Empty;

    }
}
