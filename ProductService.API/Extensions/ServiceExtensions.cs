using FluentValidation;
using MediatR;
using ProductService.Application.Behaviors;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Mapping;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Repository;
using ProductService.Infrastructure.Services;

namespace ProductService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {
            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));
            services.AddScoped<IFileService, FileService>();

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
