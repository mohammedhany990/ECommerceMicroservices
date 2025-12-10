using MediatR;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IRepository<Order> _repository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;

        public DeleteOrderCommandHandler(IRepository<Order> repository, ILogger<DeleteOrderCommandHandler> logger)
        {
            _repository = repository;
            _logger = logger;
        }

        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order. OrderId: {OrderId}", request.OrderId);

            var order = await _repository.GetByIdAsync(request.OrderId);
            if (order == null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}", request.OrderId);
                return false;
            }

            await _repository.DeleteAsync(request.OrderId);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Order deleted successfully. OrderId: {OrderId}", request.OrderId);
            return true;
        }
    }
}
