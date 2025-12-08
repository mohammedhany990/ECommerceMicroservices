using Shared.DTOs;
using Shared.Messaging;

namespace CartService.Infrastructure.Messaging
{
    public class ProductServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public ProductServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId, int timeoutMs = 30000)
        {
            var task = _rpc.Call<ApiResponse<ProductDto>>(
                routingKey: "product.get",
                message: new { ProductId = productId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;
                return response?.Data;
            }

            throw new TimeoutException("Timeout waiting for ProductService RPC response");
        }
    }
}
