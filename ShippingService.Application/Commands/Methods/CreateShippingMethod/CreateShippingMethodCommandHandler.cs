using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Methods.CreateShippingMethod
{
    public class CreateShippingMethodCommandHandler : IRequestHandler<CreateShippingMethodCommand, ShippingMethodDto>
    {
        private readonly IRepository<ShippingMethod> _repository;
        private readonly IMapper _mapper;

        public CreateShippingMethodCommandHandler(IRepository<ShippingMethod> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShippingMethodDto> Handle(CreateShippingMethodCommand request, CancellationToken cancellationToken)
        {
            var shippingMethod = _mapper.Map<ShippingMethod>(request);

            await _repository.AddAsync(shippingMethod);
            await _repository.SaveChangesAsync();

            return _mapper.Map<ShippingMethodDto>(shippingMethod);
        }
    }
}
