using Shared.DTOs;
using Shared.Messaging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Messaging
{
    public class CategoryServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public CategoryServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc ?? throw new ArgumentNullException(nameof(rpc));
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync(int timeoutMs = 15000)
        {
            var task = _rpc.Call<ApiResponse<List<CategoryDto>>>(
                routingKey: "category.getall",
                message: new { }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;
                return response?.Data ?? new List<CategoryDto>();
            }

            throw new TimeoutException("Timeout waiting for CategoryService RPC response");
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId, int timeoutMs = 15000)
        {
            var task = _rpc.Call<ApiResponse<CategoryDto>>(
                routingKey: "category.get",
                message: new { CategoryId = categoryId }
            );

            if (await Task.WhenAny(task, Task.Delay(timeoutMs)) == task)
            {
                var response = await task;
                return response?.Data;
            }

            throw new TimeoutException("Timeout waiting for CategoryService RPC response");
        }
    }
}
