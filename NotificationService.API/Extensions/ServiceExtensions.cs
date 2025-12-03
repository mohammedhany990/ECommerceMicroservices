
using FluentValidation;
using MediatR;
using NotificationService.Application.Behaviors;
using NotificationService.Application.Commands.CreateNotification;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;


namespace NotificationService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {

            services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            services.AddScoped<IEmailService, EmailService>();


            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateNotificationCommandHandler).Assembly);

            });

            services.AddValidatorsFromAssembly(typeof(CreateNotificationCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
