using Shared.DTOs;
using Shared.Messaging;

namespace ShippingService.Infrastructure.Messaging
{
    public class PaymentServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public PaymentServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc;
        }

        public async Task<PaymentResultDto?> GetPaymentAsync(Guid orderId, int timeoutMs = 15000)
        {
            var task = _rpc.CallAsync<ApiResponse<PaymentResultDto>>(
                routingKey: "payment.getByOrder",
                message: new OrderIdRequest { OrderId = orderId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;

                if (response.Success)
                    return response.Data;

                return null;
            }

            throw new TimeoutException("Timeout waiting for PaymentService RPC response");
        }


        private class OrderIdRequest
        {
            public Guid OrderId { get; set; }
        }
    }


}
