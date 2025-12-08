using FluentValidation;
using MediatR;
using Shared.Messaging;
using ShippingService.Application.Behaviors;
using ShippingService.Application.Commands.Shipments.CreateShipment;
using ShippingService.Application.Mapping;
using ShippingService.Domain.Interfaces;
using ShippingService.Infrastructure.Messaging;
using ShippingService.Infrastructure.Repositories;
using ShippingService.Infrastructure.Services;

namespace ShippingService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));
            services.AddHostedService<ShippingServiceRpcListener>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped(typeof(IShippingCostCalculator), typeof(ShippingCostCalculator));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateShipmentCommand).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(CreateShipmentCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
