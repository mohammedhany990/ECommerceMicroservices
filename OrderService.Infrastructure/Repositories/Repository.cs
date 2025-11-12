using Microsoft.EntityFrameworkCore;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Repositories
{
    public class Repository<T> : IRepository<T> where T : class
    {

        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;

        public Repository(AppDbContext dbContext)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
        }

        public async Task<T?> FirstOrDefaultAsync(Expression<Func<T, bool>> predicate, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (include != null)
                query = include(query);

            return await query.FirstOrDefaultAsync(predicate);
        }

        public async Task<IEnumerable<T>> GetAllAsync(Expression<Func<T, bool>>? predicate = null, Func<IQueryable<T>, IQueryable<T>>? include = null)
        {
            IQueryable<T> query = _dbContext.Set<T>().AsNoTracking();

            if (include != null)
                query = include(query);

            if (predicate != null)
                query = query.Where(predicate);

            return await query.ToListAsync();
        }


        public async Task<T> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);

        public async Task AddAsync(T entity) => await _dbSet.AddAsync(entity);



        public async Task<bool> DeleteAsync(Guid id)
        {
            var entity = await _dbSet.FindAsync(id);
            if (entity == null) return false;

            _dbSet.Remove(entity);
            return true;
        }

        public Task UpdateAsync(T entity)
        {
            _dbSet.Update(entity);
            return Task.CompletedTask;
        }


        public async Task SaveChangesAsync()
        {
            await _dbContext.SaveChangesAsync();
        }
    }
}
