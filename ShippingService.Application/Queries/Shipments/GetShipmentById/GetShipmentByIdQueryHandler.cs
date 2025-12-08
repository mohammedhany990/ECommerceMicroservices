using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Queries.Shipments.GetShipmentById
{
    public class GetShipmentByIdQueryHandler : IRequestHandler<GetShipmentByIdQuery, ShipmentDto>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shipment> _repository;

        public GetShipmentByIdQueryHandler(IMapper mapper, IRepository<Shipment> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<ShipmentDto> Handle(GetShipmentByIdQuery request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                throw new ArgumentException("Shipment ID cannot be empty.", nameof(request.Id));
            }
            var shipment = await _repository.GetByIdAsync(request.Id);

            if (shipment == null)
            {
                throw new KeyNotFoundException($"Shipment with ID {request.Id} not found.");
            }
            return _mapper.Map<ShipmentDto>(shipment);
        }
    }
}
