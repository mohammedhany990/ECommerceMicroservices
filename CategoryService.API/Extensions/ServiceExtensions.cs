using CategoryService.Application.Behaviors;
using CategoryService.Application.Mapping;
using CategoryService.Application.Queries.GetProducts;
using CategoryService.Domain.Interfaces;
using CategoryService.Infrastructure.Messaging;
using CategoryService.Infrastructure.Repository;
using MediatR;

using Shared.Messaging;

namespace ProductService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddHostedService<CategoryServiceRpcListener>();
            services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();
            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());
            services.AddSingleton(typeof(IRabbitMqPublisher<>), typeof(RabbitMqPublisher<>));

            services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(GetCategoriesQuery).Assembly);
            });

            //services.AddValidatorsFromAssembly(typeof().Assembly);

            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
