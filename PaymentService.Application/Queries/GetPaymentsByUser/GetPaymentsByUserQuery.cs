using MediatR;
using PaymentService.Application.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Queries.GetPaymentsByUser
{
    public class GetPaymentsByUserQuery : IRequest<List<PaymentDto>>
    {
        public Guid UserId { get; set; }
    }
}
