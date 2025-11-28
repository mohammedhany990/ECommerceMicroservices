using MediatR;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.CancelPayment
{
    public class CancelPaymentCommand : IRequest<PaymentResultDto>
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }


}
