using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Npgsql;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;

namespace ProductService.Infrastructure.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        private readonly AppDbContext _dbContext;
        private readonly DbSet<T> _dbSet;
        private readonly string _connectionString;

        public Repository(AppDbContext dbContext, IConfiguration configuration)
        {
            _dbContext = dbContext;
            _dbSet = _dbContext.Set<T>();
            _connectionString = configuration.GetConnectionString("DefaultConnection");

        }

        public async Task<bool> TryReserveStockAsync(Guid productId, int quantity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();


            using var cmd = new NpgsqlCommand(@"
                UPDATE public.""Products""
                SET ""QuantityInStock"" = ""QuantityInStock"" - @quantity,
                    ""UpdatedAt"" = NOW()
                WHERE ""Id"" = @productId AND ""QuantityInStock"" >= @quantity
                RETURNING ""Id"";
            ", connection);


            cmd.Parameters.AddWithValue("quantity", quantity);
            cmd.Parameters.AddWithValue("productId", productId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }
        public async Task<bool> ReturnStockAsync(Guid productId, int quantity)
        {
            using var connection = new NpgsqlConnection(_connectionString);
            await connection.OpenAsync();

            using var cmd = new NpgsqlCommand(@"
                    UPDATE public.""Products""
                    SET ""QuantityInStock"" = ""QuantityInStock"" + @quantity,
                        ""UpdatedAt"" = NOW()
                    WHERE ""Id"" = @productId
                    RETURNING ""Id"";
                ", connection);

            cmd.Parameters.AddWithValue("quantity", quantity);
            cmd.Parameters.AddWithValue("productId", productId);

            var result = await cmd.ExecuteScalarAsync();
            return result != null;
        }









        public async Task<IEnumerable<T>> GetAllAsync() => await _dbSet.ToListAsync();

        public async Task<T?> GetByIdAsync(Guid id) => await _dbSet.FindAsync(id);


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


        public async Task SaveChangesAsync() => await _dbContext.SaveChangesAsync();
    }
}
