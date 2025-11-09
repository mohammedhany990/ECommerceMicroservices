using CartService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace CartService.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(string userId);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(string userId);
    }
}
