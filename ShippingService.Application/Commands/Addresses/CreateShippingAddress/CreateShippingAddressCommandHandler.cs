using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Addresses.CreateShippingAddress
{
    public class CreateShippingAddressCommandHandler : IRequestHandler<CreateShippingAddressCommand, ShippingAddressDto>
    {
        private readonly IRepository<ShippingAddress> _repository;
        private readonly IMapper _mapper;

        public CreateShippingAddressCommandHandler(IRepository<ShippingAddress> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShippingAddressDto> Handle(CreateShippingAddressCommand request, CancellationToken cancellationToken)
        {
            var shippingAddress = _mapper.Map<ShippingAddress>(request);
            await _repository.AddAsync(shippingAddress);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ShippingAddressDto>(shippingAddress);
        }
    }
}
