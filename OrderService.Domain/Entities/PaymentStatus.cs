using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Paid,
        Failed,
        Cancelled,
        Refunded
    }

}
