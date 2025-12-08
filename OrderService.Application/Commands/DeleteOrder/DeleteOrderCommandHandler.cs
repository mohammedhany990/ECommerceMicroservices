using MediatR;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IRepository<Order> _repository;

        public DeleteOrderCommandHandler(IRepository<Order> repository)
        {
            _repository = repository;
        }
        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            var order = await _repository.GetByIdAsync(request.OrderId);
            if (order == null)
                return false;
            await _repository.DeleteAsync(request.OrderId);
            await _repository.SaveChangesAsync();

            return true;
        }
    }
}
