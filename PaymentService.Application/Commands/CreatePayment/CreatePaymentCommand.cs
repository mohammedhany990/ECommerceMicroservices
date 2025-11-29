using MediatR;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Commands.CreatePayment
{
    public class CreatePaymentCommand : IRequest<PaymentDto>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Currency { get; set; } = "usd";
    }

}
