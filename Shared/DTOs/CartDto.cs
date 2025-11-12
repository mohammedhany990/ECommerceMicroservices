using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class CartDto
    {
        public Guid UserId { get; set; } 
        public List<CartItemDto> Items { get; set; } = new();
        public decimal TotalPrice { get; set; }
        public decimal ShippingCost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }

}
