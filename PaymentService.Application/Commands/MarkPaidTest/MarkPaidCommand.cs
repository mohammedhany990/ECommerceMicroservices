using MediatR;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.MarkPaidTest
{
    public class MarkPaidCommand:IRequest<bool>
    {
        public Guid OrderId { get; set; }
    }
}
