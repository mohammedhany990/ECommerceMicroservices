using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public class Order
    {
        public Guid Id { get; set; }

        public Guid UserId { get; set; }

        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        public OrderStatus Status { get; set; } = OrderStatus.Pending;

        public decimal Subtotal { get; set; }
        public decimal ShippingCost { get; set; }
        public decimal TotalAmount { get; set; }

        public string? ShippingMethod { get; set; } = string.Empty;
        public string? PaymentMethod { get; set; } = string.Empty;
        public DateTime? ExpectedDeliveryDate { get; set; }

        public List<OrderItem> Items { get; set; } = new();

    }


}
