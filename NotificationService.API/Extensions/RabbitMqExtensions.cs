using NotificationService.API.HostedServices;
using Shared.Messaging;

namespace NotificationService.API.Extensions
{
    public static class RabbitMqExtensions
    {
        public static IServiceCollection AddRabbitMqServices(this IServiceCollection services)
        {

            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());


            services.AddHostedService<RabbitMqListener>();
            services.AddHostedService<NotificationSenderWorker>();


            services.AddSingleton<RpcClient>();

            return services;
        }
    }
}
