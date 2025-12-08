using Shared.DTOs;
using Shared.Messaging;

namespace OrderService.Infrastructure.Messaging
{
    public class CartServiceRpcClient
    {
        private readonly RpcClient _rpc;

        public CartServiceRpcClient(RpcClient rpc)
        {
            _rpc = rpc;
        }

        public Task<CartDto?> GetCartForUser(Guid userId)
        {
            return _rpc.Call<CartDto>(
                routingKey: "cart.get",
                message: new { UserId = userId }
            );
        }

        public async Task<bool> ClearCartForUser(Guid userId)
        {
            return await _rpc.Call<bool>(
                routingKey: "cart.clear",
                message: new { UserId = userId }
            );
        }

        public Task<CartDto?> RestoreItemsToCart(Guid userId, List<CartItemDto> items)
        {
            return _rpc.Call<CartDto>(
                routingKey: "cart.restore",
                message: new { UserId = userId, Items = items }
            );
        }
    }
}
