using FluentValidation;
using MediatR;
using PaymentService.Application.Behaviors;
using PaymentService.Application.Commands.CreatePayment;
using PaymentService.Application.Events;
using PaymentService.Application.Mapping;
using PaymentService.Domain.Interfaces;
using PaymentService.Infrastructure.Repositories;
using PaymentService.Infrastructure.Services;

namespace PaymentService.API.Extensions
{
    public static class ServiceExtensions
    {
        public static IServiceCollection AddApplicationServices(this IServiceCollection services)
        {




            services.AddScoped<IStripeWebhookService, StripeWebhookService>();
            services.AddScoped<IPaymentRepository, PaymentRepository>();

            services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);

            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreatePaymentCommand).Assembly);
            });


            services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(PaymentSucceededEventHandler).Assembly);
            });


            services.AddValidatorsFromAssembly(typeof(CreatePaymentCommandValidator).Assembly);
            services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));

            return services;
        }
    }
}
