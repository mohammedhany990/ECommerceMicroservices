using MediatR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Messaging;

namespace OrderService.Application.Commands.DeleteOrder
{
    public class DeleteOrderCommandHandler : IRequestHandler<DeleteOrderCommand, bool>
    {
        private readonly IRepository<Order> _repository;
        private readonly ILogger<DeleteOrderCommandHandler> _logger;
        private readonly ProductServiceRpcClient _productServiceRpcClient;

        public DeleteOrderCommandHandler(
            IRepository<Order> repository,
            ProductServiceRpcClient productServiceRpcClient,
            ILogger<DeleteOrderCommandHandler> logger)
        {
            _repository = repository;
            _productServiceRpcClient = productServiceRpcClient;
            _logger = logger;
        }


        public async Task<bool> Handle(DeleteOrderCommand request, CancellationToken cancellationToken)
        {
            _logger.LogInformation("Deleting order. OrderId: {OrderId}", request.OrderId);

            var order = await _repository.FirstOrDefaultAsync(
                o => o.Id == request.OrderId,
                include: o => o.Include(i => i.Items));
            if (order == null)
            {
                _logger.LogWarning("Order not found. OrderId: {OrderId}", request.OrderId);
                return false;
            }

            foreach (var item in order.Items)
            {
                bool stockReturned;
                try
                {
                    stockReturned = await _productServiceRpcClient.ReturnStockAsync(item.ProductId, item.Quantity);
                }
                catch (Exception ex)
                {
                    _logger.LogError(ex, "Error returning stock for ProductId {ProductId}", item.ProductId);
                    return false;
                }

                if (!stockReturned)
                {
                    _logger.LogWarning(
                        "Cannot delete order. Failed to return stock for ProductId {ProductId}, Quantity {Quantity}",
                        item.ProductId,
                        item.Quantity
                    );
                    return false;
                }

                _logger.LogInformation(
                    "Returned {Quantity} units to stock for ProductId {ProductId}",
                    item.Quantity,
                    item.ProductId
                );
            }

            await _repository.DeleteAsync(request.OrderId);
            await _repository.SaveChangesAsync();

            _logger.LogInformation("Order deleted successfully. OrderId: {OrderId}", request.OrderId);
            return true;

        }
    }
}
