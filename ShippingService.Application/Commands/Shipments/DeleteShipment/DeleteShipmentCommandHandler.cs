using MediatR;
using Microsoft.Extensions.Logging;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Shipments.DeleteShipment
{
    public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, bool>
    {
        private readonly IRepository<Shipment> _repository;
        private readonly ILogger<DeleteShipmentCommandHandler> _logger;

        public DeleteShipmentCommandHandler(IRepository<Shipment> repository, ILogger<DeleteShipmentCommandHandler>logger)
        {
            _repository = repository;
            _logger = logger;
        }
        public async Task<bool> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                _logger.LogWarning("DeleteShipmentCommand failed: Shipment Id is empty");
                throw new ArgumentException("Shipment Id cannot be empty.", nameof(request.Id));
            }

            var deleted = await _repository.DeleteAsync(request.Id);
            if (!deleted)
            {
                _logger.LogWarning("DeleteShipmentCommand failed: Shipment with Id {ShipmentId} not found", request.Id);
                throw new KeyNotFoundException($"Shipment with Id {request.Id} not found.");
            }

            await _repository.SaveChangesAsync();

            _logger.LogInformation("Shipment {ShipmentId} deleted successfully", request.Id);

            return true;
        }
    }
}
