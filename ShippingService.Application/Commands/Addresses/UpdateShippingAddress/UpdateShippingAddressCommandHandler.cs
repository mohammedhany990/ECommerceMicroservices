using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Addresses.UpdateShippingAddress
{
    public class UpdateShippingAddressCommandHandler
        : IRequestHandler<UpdateShippingAddressCommand, ShippingAddressDto>
    {
        private readonly IRepository<ShippingAddress> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateShippingAddressCommandHandler> _logger;

        public UpdateShippingAddressCommandHandler(
            IRepository<ShippingAddress> repository,
            IMapper mapper,
            ILogger<UpdateShippingAddressCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShippingAddressDto> Handle(UpdateShippingAddressCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Starting update for shipping address {AddressId} by UserId {UserId}", request.Id, request.UserId);

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Address Id is required.");

            var shippingAddress = await _repository.GetByIdAsync(request.Id);
            if (shippingAddress == null)
            {
                _logger.LogWarning("Shipping address {AddressId} not found", request.Id);
                throw new KeyNotFoundException($"Shipping address with Id {request.Id} not found.");
            }

            if (shippingAddress.UserId != request.UserId)
            {
                _logger.LogWarning("User {UserId} attempted to update another user's address {AddressId}", request.UserId, request.Id);
                throw new UnauthorizedAccessException("You cannot update this address.");
            }

            if (request.IsDefault)
            {
                var userAddresses = await _repository.GetAllAsync(a => a.UserId == request.UserId, cancellationToken);
                foreach (var addr in userAddresses)
                {
                    if (addr.IsDefault && addr.Id != request.Id)
                    {
                        addr.IsDefault = false;
                        _logger.LogInformation("Removed default from AddressId {AddressId}", addr.Id);
                    }
                }
            }

            _mapper.Map(request, shippingAddress);

            shippingAddress.UserId = request.UserId;

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Successfully updated shipping address {AddressId} for UserId {UserId}", shippingAddress.Id, request.UserId);

            return _mapper.Map<ShippingAddressDto>(shippingAddress);
        }
    }
}
