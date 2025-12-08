using FluentValidation;
using MediatR;
using UserService.Application.Behaviors;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.Mapping;
using UserService.Domain.Interfaces;
using UserService.Infrastructure.Configurations;
using UserService.Infrastructure.Repositories;
using UserService.Infrastructure.Services;


namespace UserService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services, IConfiguration configuration)
        {
            services.Configure<JwtSettings>(configuration.GetSection("JwtSettings"));


            services.AddScoped<IPasswordHasher, PasswordHasher>();
            services.AddScoped<ITokenService, TokenService>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(RegisterUserCommand).Assembly);
            });

            services.AddValidatorsFromAssembly(typeof(RegisterUserCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
