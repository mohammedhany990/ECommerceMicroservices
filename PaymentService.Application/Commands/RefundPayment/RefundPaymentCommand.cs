using MediatR;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.RefundPayment
{
    public class RefundPaymentCommand : IRequest<PaymentDto>
    {
        public Guid PaymentId { get; set; }
    }

}
