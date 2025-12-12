using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Addresses.CreateShippingAddress
{
    public class CreateShippingAddressCommandHandler : IRequestHandler<CreateShippingAddressCommand, ShippingAddressDto>
    {
        private readonly IRepository<ShippingAddress> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<CreateShippingAddressCommandHandler> _logger;

        public CreateShippingAddressCommandHandler(
            IRepository<ShippingAddress> repository,
            IMapper mapper,
            ILogger<CreateShippingAddressCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShippingAddressDto> Handle(CreateShippingAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Creating shipping address for UserId {UserId}", request.UserId);

            if (request.IsDefault)
            {
                var userAddresses = await _repository.GetAllAsync(a => a.UserId == request.UserId, cancellationToken);

                foreach (var addr in userAddresses)
                {
                    if (addr.IsDefault)
                    {
                        addr.IsDefault = false;
                        _logger.LogInformation("Removed default from AddressId {AddressId} for UserId {UserId}", addr.Id, request.UserId);
                    }
                }
            }

            var shippingAddress = _mapper.Map<ShippingAddress>(request);
            await _repository.AddAsync(shippingAddress);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Created shipping address {AddressId} for UserId {UserId}", shippingAddress.Id, request.UserId);

            return _mapper.Map<ShippingAddressDto>(shippingAddress);
        }
    }
}
