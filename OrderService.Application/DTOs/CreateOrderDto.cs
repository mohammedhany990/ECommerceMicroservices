using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs
{
    public class CreateOrderDto
    {
        public Guid ShippingAddressId { get; set; }
        public Guid ShippingMethodId { get; set; }
    }
}
