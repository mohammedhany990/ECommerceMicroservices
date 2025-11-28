
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using ShippingService.Infrastructure.Data;
using System;

namespace ShippingService.API.Extensions
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
