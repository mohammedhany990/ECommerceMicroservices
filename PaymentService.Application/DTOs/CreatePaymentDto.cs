using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.DTOs
{
    public class CreatePaymentDto
    {
        public Guid UserId { get; set; }
        public Guid OrderId { get; set; }
    }
}
