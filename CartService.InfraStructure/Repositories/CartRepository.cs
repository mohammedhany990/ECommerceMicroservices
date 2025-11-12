using CartService.Domain.Entities;
using CartService.Domain.Interfaces;
using StackExchange.Redis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

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

        private string GetKey(Guid userId) => $"cart:{userId}";
    }
}
