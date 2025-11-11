using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Domain.Entities
{
    public class ShippingMethod
    {
        public Guid Id { get; set; }

        public string Name { get; set; } = string.Empty;
        public decimal Cost { get; set; }
        public int EstimatedDeliveryDays { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;
    }

}
