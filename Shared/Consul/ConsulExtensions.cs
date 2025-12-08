using Consul;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace Shared.Consul
{
    public static class ConsulExtensions
    {
        public static IServiceCollection AddConsul(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<ConsulConfig>(configuration.GetSection("ServiceSettings"));

            services.AddSingleton<IConsulClient, ConsulClient>(p =>
                new ConsulClient(cfg =>
                {
                    cfg.Address = new Uri("http://localhost:8500");
                }));

            services.AddSingleton<IHostedService, ConsulHostedService>();

            return services;
        }
    }

}
