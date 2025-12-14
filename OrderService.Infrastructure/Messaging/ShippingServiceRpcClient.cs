using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Messaging
{
    public class ShippingServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public ShippingServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        public async Task<ShippingCostResultDto> CalculateShippingCostAsync(ShippingCostRequestDto dto, int timeoutMs = 5000)
        {
            var task = _rpc.CallAsync<ShippingCostResultDto>(
                routingKey: "shipping.calculate",
                message: dto
            );

            var completedTask = await Task.WhenAny(task, Task.Delay(timeoutMs));

            if (completedTask != task)
                throw new TimeoutException("Timeout waiting for ShippingService RPC response");

            var result = await task;

            if (result == null)
                throw new Exception("Shipping service returned no result");


            return result;
        }


        public async Task<ShippingMethodDto> GetShippingMethodByIdAsync(Guid id, int timeoutMs = 5000)
        {
            var task = _rpc.CallAsync<ShippingMethodDto>(
                routingKey: "shipping.get",
                message: new { Id = id }
            );

            var completedTask = await Task.WhenAny(task, Task.Delay(timeoutMs));

            if (completedTask != task)
                throw new TimeoutException("Timeout waiting for ShippingService RPC response");

            var result = await task;

            if (result == null)
                throw new Exception($"Shipping method with ID {id} not found");

            return result;
        }
    }
}
