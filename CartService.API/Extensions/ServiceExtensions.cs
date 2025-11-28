using CartService.Application.Behaviors;
using CartService.Application.Commands.AddItemToCart;
using CartService.Application.Mapping;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.MessageBus;
using CartService.InfraStructure.MessageBus;
using CartService.InfraStructure.Repositories;
using FluentValidation;
using MediatR;


namespace CartService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

            services.AddHostedService<CartServiceRpcListener>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(ICartRepository), typeof(CartRepository));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(AddItemToCartCommand).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(AddItemToCartCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
