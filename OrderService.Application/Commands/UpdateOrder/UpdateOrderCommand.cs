using MediatR;
using OrderService.Application.DTOs;
using OrderService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.Commands.UpdateOrder
{
    public class UpdateOrderCommand:IRequest<OrderDto>
    {
        public Guid OrderId { get; set; }
        public Guid? ShippingAddressId { get; set; }
        public Guid? ShippingMethodId { get; set; }
        public string? PaymentMethod { get; set; } = string.Empty;
        public DateTime? ExpectedDeliveryDate { get; set; }
        public OrderStatus? Status { get; set; }

        public string? AuthToken { get; set; } = string.Empty;


    }
}
