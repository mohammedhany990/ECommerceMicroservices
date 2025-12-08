using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using StackExchange.Redis;
using System.Text.Json;

namespace CartService.InfraStructure.Repositories
{
    public class CartRepository : ICartRepository
    {
        private readonly IDatabase _database;

        public CartRepository(IConnectionMultiplexer redis)
        {
            _database = redis.GetDatabase();
        }

        public async Task<Cart?> GetCartAsync(Guid userId)
        {
            var data = await _database.StringGetAsync(GetKey(userId));

            return data.IsNullOrEmpty ? null : JsonSerializer.Deserialize<Cart>(data!);
        }

        public async Task<Cart> UpdateCartAsync(Cart cart)
        {
            await _database.StringSetAsync(GetKey(cart.UserId),
                JsonSerializer.Serialize(cart),
                TimeSpan.FromDays(7));

            return cart;
        }

        public async Task<bool> DeleteCartAsync(Guid userId)
        {
            return await _database.KeyDeleteAsync(GetKey(userId));
        }


        public async Task<bool> ClearCartAsync(Guid userId)
        {
            return await _database.KeyDeleteAsync(GetKey(userId));
        }

        public async Task<Cart> RestoreItemsAsync(Guid userId, List<CartItem> items)
        {
            var cart = await GetCartAsync(userId) ?? new Cart { UserId = userId, Items = new List<CartItem>() };

            foreach (var item in items)
            {
                var existingItem = cart.Items.FirstOrDefault(i => i.ProductId == item.ProductId);
                if (existingItem != null)
                    existingItem.Quantity += item.Quantity;
                else
                    cart.Items.Add(item);
            }

            await UpdateCartAsync(cart);
            return cart;
        }


        private string GetKey(Guid userId) => $"cart:{userId}";
    }
}
