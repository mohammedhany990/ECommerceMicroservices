using StackExchange.Redis;

namespace CartService.API.Extensions
{
    public static class DatabaseExtensions
    {
        public static IServiceCollection AddDatabase(this IServiceCollection services, IConfiguration configuration)
        {

            services.AddSingleton<IConnectionMultiplexer>(serviceProvider =>
            {
                var redisConnection = configuration.GetConnectionString("Redis");
                return ConnectionMultiplexer.Connect(redisConnection);
            });

            return services;
        }
    }
}
