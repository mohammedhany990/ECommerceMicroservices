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
            return await _rpcClient.Call<OrderDto>("order.request", orderId);
        }
    }
}
