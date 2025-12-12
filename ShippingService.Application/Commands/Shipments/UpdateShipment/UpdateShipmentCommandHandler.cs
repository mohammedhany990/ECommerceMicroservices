using AutoMapper;
using MediatR;
using Microsoft.Extensions.Logging;
using Shared.Enums;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Shipments.UpdateShipment
{
    public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, ShipmentDto>
    {
        private readonly IRepository<Shipment> _repository;
        private readonly IMapper _mapper;
        private readonly ILogger<UpdateShipmentCommandHandler> _logger;

        public UpdateShipmentCommandHandler(
            IRepository<Shipment> repository,
            IMapper mapper,
            ILogger<UpdateShipmentCommandHandler> logger)
        {
            _repository = repository;
            _mapper = mapper;
            _logger = logger;
        }

        public async Task<ShipmentDto> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            if (request == null)
            {
                _logger.LogError("UpdateShipmentCommand request is null");
                throw new ArgumentNullException(nameof(request));
            }

            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("UpdateShipmentCommand failed: Shipment Id is empty");
                throw new ArgumentException("Shipment Id cannot be empty.");
            }

            _logger.LogInformation("Fetching shipment with Id {ShipmentId}", request.Id);
            var shipment = await _repository.GetByIdAsync(request.Id);
            if (shipment == null)
            {
                _logger.LogWarning("Shipment with Id {ShipmentId} not found", request.Id);
                throw new KeyNotFoundException($"Shipment with Id {request.Id} not found.");
            }

            if (!string.IsNullOrEmpty(request.Status))
            {
                if (Enum.TryParse<ShipmentStatus>(request.Status, ignoreCase: true, out var parsedStatus))
                {
                    shipment.Status = parsedStatus;
                    _logger.LogInformation("Shipment {ShipmentId} status set to {Status}", shipment.Id, shipment.Status);
                }
                else
                {
                    _logger.LogWarning("Invalid shipment status '{Status}' for ShipmentId {ShipmentId}", request.Status, shipment.Id);
                    throw new ArgumentException($"Invalid shipment status: {request.Status}");
                }
            }

            _mapper.Map(request, shipment);

            shipment.UpdatedAt = DateTime.UtcNow;
            _logger.LogInformation("Saving changes for shipment {ShipmentId}", shipment.Id);

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Shipment {ShipmentId} updated successfully", shipment.Id);

            return _mapper.Map<ShipmentDto>(shipment);
        }
    }
}
