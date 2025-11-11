using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Queries.Addresses.GetShippingAddresses
{
    public class GetShippingAddressesQueryHandler : IRequestHandler<GetShippingAddressesQuery, List<ShippingAddressDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ShippingAddress> _repository;

        public GetShippingAddressesQueryHandler(IMapper mapper, IRepository<ShippingAddress> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<List<ShippingAddressDto>> Handle(GetShippingAddressesQuery request, CancellationToken cancellationToken)
        {
            var shippingAddresses = await _repository.GetAllAsync();
            return _mapper.Map<List<ShippingAddressDto>>(shippingAddresses);
        }
    }
}
