using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NotificationService.Infrastructure.Messaging
{
    public class UserServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public UserServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }
        public async Task<bool> UserExistsAsync(Guid userId, int timeoutMs = 15000)
        {
            var task = _rpc.Call<ApiResponse<bool>>(
                routingKey: "user.exists",
                message: new { UserId = userId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;
                return response?.Data ?? false;
            }

            throw new TimeoutException("Timeout waiting for UserService RPC response");
        }
    }
}
