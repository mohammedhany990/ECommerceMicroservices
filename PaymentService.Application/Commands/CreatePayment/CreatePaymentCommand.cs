using MediatR;
using PaymentService.Application.DTOs;

namespace PaymentService.Application.Commands.CreatePayment
{
    public class CreatePaymentCommand : IRequest<PaymentDto>
    {
        public Guid OrderId { get; set; }
        public Guid UserId { get; set; }
        public string Currency { get; set; } = "usd";
    }

}
