using CartService.Infrastructure.Messaging;
using CartService.InfraStructure.Messaging;
using Shared.Messaging;

namespace CartService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddHostedService<CartServiceRpcListener>();
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddSingleton<ShippingServiceRpcClient>();
            services.AddSingleton<ProductServiceRpcClient>();

            services.AddSingleton<RpcClient>();

            return services;
        }
    }
}
