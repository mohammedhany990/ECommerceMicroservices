using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using OrderService.Application.Behaviors;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Mapping;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Interfaces;
using OrderService.Infrastructure.MessageBus;
using OrderService.Infrastructure.Repositories;

namespace OrderService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<OrderServiceRpcListener>();
            services.AddSingleton<CartServiceRpcClient>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommand).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
