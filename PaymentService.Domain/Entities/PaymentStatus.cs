using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Entities
{
    public enum PaymentStatus
    {
        Pending,
        Succeeded,
        Processing,
        Failed,
        Refunded,
        Canceled,
        Confirmed
    }

}
