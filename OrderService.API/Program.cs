using Microsoft.EntityFrameworkCore;
using OrderService.API.Extensions;
using OrderService.API.Middlewares;
using Shared.Consul;
namespace OrderService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            builder.Services.AddEndpointsApiExplorer();

            builder.Services
               .AddSwaggerWithJwt()
               .AddApplicationServices()
               .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!)
               .AddJwtAuthentication(builder.Configuration)
               .ConfigureApiBehavior()
               .AddCustomRateLimiting()
               .AddRabbitMqServices()
               .AddCustomHealthChecks(builder.Configuration);

            SerilogBootstrap.ConfigureSerilog(builder);

            builder.Services.AddConsul(builder.Configuration);


            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseRateLimiter();
            app.UseHttpsRedirection();
            app.UseRouting();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseHealthChecksEndpoints();
            app.MapControllers();
            app.MapGet("/health", () => "Healthy");

            app.Run();
        }
    }
}
