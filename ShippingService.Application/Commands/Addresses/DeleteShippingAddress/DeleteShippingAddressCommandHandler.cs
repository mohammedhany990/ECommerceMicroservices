using MediatR;
using ShippingService.Domain.Entities;
using ShippingService.Domain.Interfaces;

namespace ShippingService.Application.Commands.Addresses.DeleteShippingAddress
{
    public class DeleteShippingAddressCommandHandler : IRequestHandler<DeleteShippingAddressCommand, bool>
    {
        private readonly IRepository<ShippingAddress> _repository;

        public DeleteShippingAddressCommandHandler(IRepository<ShippingAddress> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteShippingAddressCommand request, CancellationToken cancellationToken)
        {
            if (request.Id == Guid.Empty)
            {
                throw new ArgumentException("Id cannot be empty", nameof(request.Id));
            }

            var deleted = await _repository.DeleteAsync(request.Id);
            if (!deleted)
                throw new KeyNotFoundException($"Shipping address with Id {request.Id} not found");

            await _repository.SaveChangesAsync();
            return true;
        }
    }
}
