using Shared.DTOs;
using Shared.Messaging;

namespace OrderService.Infrastructure.Messaging
{
    public class ShippingServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public ShippingServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc;
        }

        public async Task<ShippingCostResultDto?> CalculateShippingCostAsync(ShippingCostRequestDto dto, int timeoutMs = 5000)
        {
            var task = _rpc.Call<ShippingCostResultDto>(
                routingKey: "shipping.calculate",
                message: dto
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                return await task;
            else
                throw new TimeoutException("Timeout waiting for ShippingService RPC response");
        }

        public async Task<ShippingMethodDto?> GetShippingMethodByIdAsync(Guid id, int timeoutMs = 5000)
        {
            var task = _rpc.Call<ShippingMethodDto>(
                routingKey: "shipping.get",
                message: new { Id = id }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                return await task;
            else
                throw new TimeoutException("Timeout waiting for ShippingService RPC response");
        }
    }

}
