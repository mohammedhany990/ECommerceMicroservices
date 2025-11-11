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

namespace ShippingService.Application.Commands.Shipments.UpdateShipment
{
    public class UpdateShipmentCommandHandler : IRequestHandler<UpdateShipmentCommand, ShipmentDto>
    {
        private readonly IRepository<Shipment> _repository;
        private readonly IMapper _mapper;

        public UpdateShipmentCommandHandler(IRepository<Shipment> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShipmentDto> Handle(UpdateShipmentCommand request, CancellationToken cancellationToken)
        {
            if(request.Id == Guid.Empty)
            {
                throw new ArgumentException("Shipment Id cannot be empty.");
            }
            var shipment = await _repository.GetByIdAsync(request.Id);
            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with Id {request.Id} not found.");
            }

            _mapper.Map(request, shipment);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ShipmentDto>(shipment);
        }
    }
}
