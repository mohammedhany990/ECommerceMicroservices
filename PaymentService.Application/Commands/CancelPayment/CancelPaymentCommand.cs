using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands.CancelPayment
{
    public class CancelPaymentCommand : IRequest<PaymentResultDto>
    {
        public string PaymentIntentId { get; set; } = string.Empty;
    }


}
