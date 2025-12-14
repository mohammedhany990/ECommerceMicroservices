using Shared.DTOs;
using Shared.Messaging;

namespace PaymentService.Infrastructure.Messaging
{
    public class UserServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public UserServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        public async Task<string?> GetUserEmailAsync(Guid userId, int timeoutMs = 15000)
        {
            var task = _rpc.CallAsync<ApiResponse<string>>(
                routingKey: "user.getemail",
                message: new { UserId = userId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;
                return response?.Data;
            }

            throw new TimeoutException("Timeout waiting for UserService RPC response");
        }
    }
}
