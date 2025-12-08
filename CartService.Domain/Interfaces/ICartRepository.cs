using CartService.Domain.Entities;

namespace CartService.Domain.Interfaces
{
    public interface ICartRepository
    {
        Task<Cart?> GetCartAsync(Guid userId);
        Task<Cart> UpdateCartAsync(Cart cart);
        Task<bool> DeleteCartAsync(Guid userId);

        Task<Cart> RestoreItemsAsync(Guid userId, List<CartItem> items);
        Task<bool> ClearCartAsync(Guid userId);
    }
}
