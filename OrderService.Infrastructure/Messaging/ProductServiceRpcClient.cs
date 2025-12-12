using Microsoft.Extensions.Configuration;
using Npgsql;
using Shared.DTOs;
using Shared.Messaging;

namespace OrderService.Infrastructure.Messaging
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

        
        public async Task<bool> ReserveStockAsync(Guid productId, int quantity)
        {
            var task = _rpc.Call<object>(routingKey: "product.reserve_stock", message: new { ProductId = productId, Quantity = quantity });
            var response = await task;
            var json = System.Text.Json.JsonSerializer.Serialize(response);
            var obj = System.Text.Json.JsonSerializer.Deserialize<ReserveStockResponseDto>(json);
            return obj?.Success ?? false;
        }
        public async Task<bool> ReturnStockAsync(Guid productId, int quantity)
        {
            var task = _rpc.Call<object>(
                routingKey: "product.return_stock", 
                message: new { ProductId = productId, Quantity = quantity }
            );

            var response = await task;
            var json = System.Text.Json.JsonSerializer.Serialize(response);
            var obj = System.Text.Json.JsonSerializer.Deserialize<ReturnStockResponseDto>(json);
            return obj?.Success ?? false;
        }

        private record ReturnStockResponseDto(bool Success);
        private record ReserveStockResponseDto(bool Success);

    }
}
