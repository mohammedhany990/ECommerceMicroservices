
using Shared.Messaging;
using ShippingService.Infrastructure.Messaging;

namespace ShippingService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<ShippingServiceRpcListener>();

            services.AddSingleton<OrderServiceRpcClient>();
            services.AddSingleton<PaymentServiceRpcClient>();

            services.AddSingleton<RpcClient>();

            return services;
        }
    }
}
