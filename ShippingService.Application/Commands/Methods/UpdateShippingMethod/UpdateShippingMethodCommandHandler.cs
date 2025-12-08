using AutoMapper;
using MediatR;
using ShippingService.Application.DTOs;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Methods.UpdateShippingMethod
{
    public class UpdateShippingMethodCommandHandler : IRequestHandler<UpdateShippingMethodCommand, ShippingMethodDto>
    {
        private readonly IRepository<ShippingMethod> _repository;
        private readonly IMapper _mapper;

        public UpdateShippingMethodCommandHandler(IRepository<ShippingMethod> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }
        public async Task<ShippingMethodDto> Handle(UpdateShippingMethodCommand request, CancellationToken cancellationToken)
        {
            var shippingMethod = await _repository.GetByIdAsync(request.Id);

            if (shippingMethod is null)
            {
                throw new KeyNotFoundException($"Shipping Method with Id {request.Id} not found.");
            }

            _mapper.Map(request, shippingMethod);

            await _repository.SaveChangesAsync();

            return _mapper.Map<ShippingMethodDto>(shippingMethod);
        }
    }
}
