
using Microsoft.EntityFrameworkCore;
using OrderService.Infrastructure.Data;

namespace OrderService.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, string connectionString)
        {
            services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(connectionString));

            return services;
        }

    }
}
