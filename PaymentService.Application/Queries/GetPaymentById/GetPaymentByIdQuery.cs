using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Queries.GetPaymentById
{

    public class GetPaymentByIdQuery : IRequest<PaymentDto>
    {
        public Guid PaymentId { get; set; }
    }

}
