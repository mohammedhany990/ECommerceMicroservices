using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Messaging
{
    public class PaymentServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public PaymentServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc;
        }

        public async Task<PaymentResultDto?> GetPaymentStatusAsync(Guid orderId, int timeoutMs = 15000)
        {
            var task = _rpc.Call<PaymentResultDto>(
                routingKey: "payment.getByOrder",
                message: new OrderIdRequest{ OrderId = orderId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
                return await task;

            throw new TimeoutException("Timeout waiting for PaymentService RPC response");
        }

        private class OrderIdRequest
        {
            public Guid OrderId { get; set; }
        }
    }


}
