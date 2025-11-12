using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class ShippingCostResultDto
    {
        public decimal Cost { get; set; }
        public int EstimatedDeliveryDays { get; set; }
    }
}
