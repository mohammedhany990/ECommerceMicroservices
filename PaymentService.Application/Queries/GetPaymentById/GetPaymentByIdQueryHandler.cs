using AutoMapper;
using MediatR;
using PaymentService.Application.DTOs;
using PaymentService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Application.Queries.GetPaymentById
{
    public class GetPaymentByIdQueryHandler : IRequestHandler<GetPaymentByIdQuery, PaymentDto>
    {
        private readonly IPaymentRepository _repository;
        private readonly IMapper _mapper;

        public GetPaymentByIdQueryHandler(IPaymentRepository repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<PaymentDto> Handle(GetPaymentByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.PaymentId == Guid.Empty)
                throw new ArgumentException("PaymentId cannot be empty.", nameof(request.PaymentId));

            var payment = await _repository.GetByIdAsync(request.PaymentId);

            if (payment == null)
                throw new KeyNotFoundException($"Payment with ID {request.PaymentId} not found.");

            return _mapper.Map<PaymentDto>(payment);
        }
    }
}
