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

namespace ShippingService.Application.Queries.Addresses.GetShippingAddressById
{
    public class GetShippingAddressByIdQueryHandler : IRequestHandler<GetShippingAddressByIdQuery, ShippingAddressDto>
    {
        private readonly IRepository<ShippingAddress> _repository;
        private readonly IMapper _mapper;

        public GetShippingAddressByIdQueryHandler(IRepository<ShippingAddress> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShippingAddressDto> Handle(GetShippingAddressByIdQuery request, CancellationToken cancellationToken)
        {
            if(request.ID == Guid.Empty)
            {
                throw new ArgumentException("ID cannot be empty", nameof(request.ID));
            }

            var shippingAddress = await _repository.GetByIdAsync(request.ID);

            if (shippingAddress is null)
            {
                throw new KeyNotFoundException($"Shipping address with ID {request.ID} not found.");
            }
            return _mapper.Map<ShippingAddressDto>(shippingAddress);
        }
    }
}
