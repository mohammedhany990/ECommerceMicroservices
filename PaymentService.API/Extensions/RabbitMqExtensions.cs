using PaymentService.Infrastructure.Messaging;
using Shared.Messaging;
using PaymentService.Infrastructure.Messaging;

namespace PaymentService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<PaymentServiceRpcListener>();
            services.AddSingleton<OrderServiceRpcClient>();
            services.AddSingleton<UserServiceRpcClient>();
            services.AddSingleton<RpcClient>();



            return services;
        }
    }
}
