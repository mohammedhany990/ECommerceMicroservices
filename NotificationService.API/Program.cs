
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.OpenApi.Models;
using NotificationService.API.HostedServices;
using NotificationService.API.Middlewares;
using NotificationService.API.Models.Responses;
using NotificationService.Application.Behaviors;
using NotificationService.Application.Commands.CreateNotification;
using NotificationService.Domain.Interfaces;
using NotificationService.Infrastructure.Data;
using NotificationService.Infrastructure.Repositories;
using NotificationService.Infrastructure.Services;
using Shared.Messaging;

namespace NotificationService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            
            #region Swagger
            builder.Services.AddSwaggerGen(opt =>
            {

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                opt.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                            { securitySchema, new[] { "Bearer" } }
                };

                opt.AddSecurityRequirement(securityRequirement);

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Notification Service", Version = "v1.0" });

            });
            #endregion


            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateNotificationCommandHandler).Assembly);

            });
            builder.Services.AddValidatorsFromAssembly(typeof(CreateNotificationCommandValidator).Assembly);


            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));




            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped(typeof(IRepository<>),typeof(Repository<>));


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
                        .Select(m => m.ErrorMessage)
                        .ToList();

                    var response = ApiResponse<object>.FailResponse(errors, "Validation failed", 400);

                    return new BadRequestObjectResult(response);
                };
            });

            // Add RabbitMQ connection singleton
            builder.Services.AddSingleton<IRabbitMqConnection, RabbitMqConnection>();

            // Add IModel singleton resolved via the connection
            builder.Services.AddSingleton(sp => sp.GetRequiredService<IRabbitMqConnection>().CreateChannel());


            builder.Services.AddScoped<IEmailService, EmailService>();
            builder.Services.AddHostedService<RabbitMqListener>();
            builder.Services.AddHostedService<NotificationSenderWorker>();



            var app = builder.Build();

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();

            app.UseAuthorization();


            app.MapControllers();

            app.Run();
        }
    }
}
