using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries.GetPaymentsByUser
{
    public class GetPaymentsByUserQuery : IRequest<List<PaymentDto>>
    {
        public Guid UserId { get; set; }

    }
}
