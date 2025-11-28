using MediatR;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Queries.GetPaymentByOrderId
{
    public class GetPaymentByOrderIdQuery : IRequest<PaymentResultDto>
    {
        public Guid OrderId { get; set; }

        public GetPaymentByOrderIdQuery(Guid orderId)
        {
            OrderId = orderId;
        }
    }
}
