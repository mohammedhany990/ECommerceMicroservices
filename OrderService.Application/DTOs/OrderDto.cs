using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs
{
    public class OrderDto
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }
        public Guid ShippingAddressId { get; set; }
        public DateTime CreatedAt { get; set; }

        public string Status { get; set; } = string.Empty;

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public string ShippingMethod { get; set; } = string.Empty;
        public string PaymentMethod { get; set; } = string.Empty;

        public List<OrderItemDto> Items { get; set; } = new();
    }

}
