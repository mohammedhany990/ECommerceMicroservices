using System.Linq.Expressions;

namespace UserService.Domain.Interfaces
{
    public interface IRepository<T> where T : class
    {
        Task<string> GetUserEmailAsync(Guid userId);
        Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate);
        Task<T?> GetByIdAsync(Guid id);
        Task<IEnumerable<T>> GetAllAsync();
        Task AddAsync(T entity);
        Task UpdateAsync(T entity);
        Task DeleteAsync(Guid id);
        Task SaveChangesAsync();
    }

}
