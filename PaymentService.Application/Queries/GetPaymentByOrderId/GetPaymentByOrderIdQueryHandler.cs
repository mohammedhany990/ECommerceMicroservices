using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Application.Queries.GetPaymentByOrderId
{
    public class GetPaymentByOrderIdQueryHandler : IRequestHandler<GetPaymentByOrderIdQuery, PaymentResultDto>
    {
        private readonly IPaymentRepository _paymentRepository;

        public GetPaymentByOrderIdQueryHandler(IPaymentRepository paymentRepository)
        {
            _paymentRepository = paymentRepository;
        }

        public async Task<PaymentResultDto> Handle(GetPaymentByOrderIdQuery request, CancellationToken cancellationToken)
        {
            if (request.OrderId == Guid.Empty)
            {
                throw new ArgumentException("OrderId cannot be empty.", nameof(request.OrderId));
            }
            var payment = await _paymentRepository.GetByOrderIdAsync(request.OrderId);

            if (payment is null)
                return null!;

            var result = new PaymentResultDto
            {
                PaymentId = payment.Id,
                OrderId = payment.OrderId,
                Amount = payment.Amount,
                Currency = payment.Currency,
                Status = payment.Status.ToString(),
                ConfirmedAt = payment.ConfirmedAt,
                FailureReason = payment.FailureReason,
                PaymentIntentId = payment.PaymentIntentId

            };

            return result;
        }
    }
}
