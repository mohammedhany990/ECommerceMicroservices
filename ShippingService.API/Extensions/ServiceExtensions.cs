using FluentValidation;
using MediatR;
using Microsoft.Extensions.DependencyInjection;
using ShippingService.Application.Behaviors;
using ShippingService.Application.Commands.Shipments.CreateShipment;
using ShippingService.Application.Mapping;
using ShippingService.Domain.Interfaces;
using ShippingService.Infrastructure.Repositories;

namespace ShippingService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

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
