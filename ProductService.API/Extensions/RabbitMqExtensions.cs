
using ProductService.Infrastructure.Messaging;
using Shared.Messaging;

namespace ProductService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddSingleton<RpcClient>();
            services.AddHostedService<ProductServiceRpcListener>();
            services.AddSingleton<CategoryServiceRpcClient>();
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));



            return services;
        }
    }
}
