using OrderService.Infrastructure.Messaging;
using PaymentService.Infrastructure.Messaging;
using Shared.Messaging;

namespace OrderService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<OrderServiceRpcListener>();
            services.AddSingleton<ProductServiceRpcClient>();
            services.AddSingleton<CartServiceRpcClient>();
            services.AddSingleton<ShippingServiceRpcClient>();
            services.AddSingleton<PaymentServiceRpcClient>();
            services.AddSingleton<UserServiceRpcClient>();

            services.AddSingleton<RpcClient>();

            return services;
        }
    }
}
