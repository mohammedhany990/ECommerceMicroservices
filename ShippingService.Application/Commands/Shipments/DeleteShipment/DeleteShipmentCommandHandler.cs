using MediatR;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Shipments.DeleteShipment
{
    public class DeleteShipmentCommandHandler : IRequestHandler<DeleteShipmentCommand, bool>
    {
        private readonly IRepository<Shipment> _repository;

        public DeleteShipmentCommandHandler(IRepository<Shipment> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteShipmentCommand request, CancellationToken cancellationToken)
        {
            if(request.Id == Guid.Empty)
            {
                throw new ArgumentException("Shipment Id cannot be empty.", nameof(request.Id));
            }

            var deleted = await _repository.DeleteAsync(request.Id);
            if (!deleted)
                throw new KeyNotFoundException($"Shipment with Id {request.Id} not found.");

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
