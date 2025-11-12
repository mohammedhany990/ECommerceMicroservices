using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Shared.DTOs
{
    public class ShippingCostRequestDto
    {
        public ShippingCostRequestDto(Guid shippingAddressId, Guid shippingMethodId)
        {
            ShippingAddressId = shippingAddressId;
            ShippingMethodId = shippingMethodId;
        }

        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }

    }
}
