using Shared.Messaging;
using UserService.Infrastructure.Messaging;

namespace OrderService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<UserServiceRpcListener>();


            services.AddSingleton<RpcClient>();

            return services;
        }
    }
}
