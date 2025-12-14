using Shared.DTOs;
using Shared.Messaging;

namespace ShippingService.Infrastructure.Messaging
{
    public class OrderServiceRpcClient
    {
        private readonly RpcClient _rpcClient;

        public OrderServiceRpcClient(RpcClient rpcClient)
        {
            _rpcClient = rpcClient;
        }

        public async Task<OrderDto?> GetOrderByIdAsync(Guid orderId)
        {
            return await _rpcClient.CallAsync<OrderDto>("order.request", orderId);
        }
        public async Task<bool> UpdateOrderStatusAsync(Guid orderId, string newStatus)
        {
            var result = await _rpcClient.CallAsync<bool>("order.updateStatus", new { OrderId = orderId, Status = newStatus });
            return result;
        }

    }
}
