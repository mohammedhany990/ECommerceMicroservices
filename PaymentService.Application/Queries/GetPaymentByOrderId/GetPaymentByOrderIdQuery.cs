using MediatR;
using PaymentService.Application.DTOs;

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
