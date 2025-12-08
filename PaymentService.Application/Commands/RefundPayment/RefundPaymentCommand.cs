using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands.RefundPayment
{
    public class RefundPaymentCommand : IRequest<PaymentDto>
    {
        public Guid PaymentId { get; set; }
    }

}
