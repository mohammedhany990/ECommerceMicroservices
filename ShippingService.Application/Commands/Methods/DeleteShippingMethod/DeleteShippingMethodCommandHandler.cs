using MediatR;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ShippingService.Application.Commands.Methods.DeleteShippingMethod
{
    public class DeleteShippingMethodCommandHandler : IRequestHandler<DeleteShippingMethodCommand, bool>
    {
        private readonly IRepository<ShippingMethod> _repository;

        public DeleteShippingMethodCommandHandler(IRepository<ShippingMethod> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteShippingMethodCommand request, CancellationToken cancellationToken)
        {

            if (request.Id == Guid.Empty)
                throw new ArgumentException("Shipping Method Id is required.", nameof(request.Id));

            var deleted = await _repository.DeleteAsync(request.Id);

            if (!deleted)
                throw new KeyNotFoundException($"Shipping Method with Id {request.Id} not found.");

            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
