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

namespace ShippingService.Application.Queries.Shipments.GetShipments
{
    public class GetShipmentsQueryHandler : IRequestHandler<GetShipmentsQuery, List<ShipmentDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<Shipment> _repository;

        public GetShipmentsQueryHandler(IMapper mapper, IRepository<Shipment> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<List<ShipmentDto>> Handle(GetShipmentsQuery request, CancellationToken cancellationToken)
        {
            var shipments = await _repository.GetAllAsync();
            return _mapper.Map<List<ShipmentDto>>(shipments);
        }
    }
}
