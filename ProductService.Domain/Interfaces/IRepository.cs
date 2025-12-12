namespace ProductService.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<bool> TryReserveStockAsync(Guid productId, int quantity);
        Task<bool> ReturnStockAsync(Guid productId, int quantity);

        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task<bool> DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }
}
