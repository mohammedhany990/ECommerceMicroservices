using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Queries.Methods.GetShippingMethodById
{
    public class GetShippingMethodByIdQueryHandler : IRequestHandler<GetShippingMethodByIdQuery, ShippingMethodDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ShippingMethod> _repository;

        public GetShippingMethodByIdQueryHandler(IMapper mapper, IRepository<ShippingMethod> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<ShippingMethodDto> Handle(GetShippingMethodByIdQuery request, CancellationToken cancellationToken)
        {
            if(request.Id == Guid.Empty)
            {
                throw new ArgumentException("Shipping Method Id is required.", nameof(request.Id));
            }
            var shippingMethod =await _repository.GetByIdAsync(request.Id);
            if(shippingMethod == null)
            {
                throw new KeyNotFoundException($"Shipping Method with Id {request.Id} not found.");
            }
            return _mapper.Map<ShippingMethodDto>(shippingMethod);
        }
    }
}
