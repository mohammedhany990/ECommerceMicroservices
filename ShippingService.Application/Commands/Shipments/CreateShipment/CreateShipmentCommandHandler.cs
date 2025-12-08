using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Shipments.CreateShipment
{
    public class CreateShipmentCommandHandler : IRequestHandler<CreateShipmentCommand, ShipmentDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shipment> _repository;

        public CreateShipmentCommandHandler(IMapper mapper, IRepository<Shipment> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<ShipmentDto> Handle(CreateShipmentCommand request, CancellationToken cancellationToken)
        {
            var shipment = _mapper.Map<Shipment>(request);
            await _repository.AddAsync(shipment);
            await _repository.SaveChangesAsync();
            return _mapper.Map<ShipmentDto>(shipment);
        }
    }
}
