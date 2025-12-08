using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Queries.Methods.GetShippingMethods
{
    public class GetShippingMethodsQueryHandler : IRequestHandler<GetShippingMethodsQuery, List<ShippingMethodDto>>
    {
        private readonly IMapper _mapper;
        private readonly IRepository<ShippingMethod> _repository;

        public GetShippingMethodsQueryHandler(IMapper mapper, IRepository<ShippingMethod> repository)
        {
            _mapper = mapper;
            _repository = repository;
        }
        public async Task<List<ShippingMethodDto>> Handle(GetShippingMethodsQuery request, CancellationToken cancellationToken)
        {
            var shippingMethods = await _repository.GetAllAsync();
            return _mapper.Map<List<ShippingMethodDto>>(shippingMethods);
        }
    }
}
