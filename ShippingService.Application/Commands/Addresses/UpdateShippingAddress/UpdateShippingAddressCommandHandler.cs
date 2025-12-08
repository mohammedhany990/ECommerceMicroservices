using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Addresses.UpdateShippingAddress
{
    public class UpdateShippingAddressCommandHandler : IRequestHandler<UpdateShippingAddressCommand, ShippingAddressDto>
    {
        private readonly IRepository<ShippingAddress> _repository;
        private readonly IMapper _mapper;

        public UpdateShippingAddressCommandHandler(IRepository<ShippingAddress> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShippingAddressDto> Handle(UpdateShippingAddressCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                throw new ArgumentException("Address Id is required.");
            }

            var shippingAddress = await _repository.GetByIdAsync(request.Id);
            if (shippingAddress == null)
            {
                throw new KeyNotFoundException($"Shipping address with Id {request.Id} not found.");
            }

            _mapper.Map(request, shippingAddress);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ShippingAddressDto>(shippingAddress);
        }
    }
}
