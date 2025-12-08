using AutoMapper;
using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Interfaces;

namespace PaymentService.Application.Queries.GetPaymentsByUser
{
    public class GetPaymentsByUserQueryHandler : IRequestHandler<GetPaymentsByUserQuery, List<PaymentDto>>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;

        public GetPaymentsByUserQueryHandler(IPaymentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<List<PaymentDto>> Handle(GetPaymentsByUserQuery request, CancellationToken cancellationToken)
        {
            var payments = await _repository.GetByUserIdAsync(request.UserId);

            if (payments == null || !payments.Any())
                return new List<PaymentDto>();

            return _mapper.Map<List<PaymentDto>>(payments);
        }
    }
}
