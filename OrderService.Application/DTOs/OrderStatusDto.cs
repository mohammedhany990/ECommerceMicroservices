using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Application.DTOs
{
    public class OrderStatusDto
    {
        public Guid Id { get; set; }
        public string Name { get; set; } = string.Empty;
    }
}
