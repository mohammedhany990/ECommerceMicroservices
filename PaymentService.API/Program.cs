using PaymentService.API.Extensions;
using PaymentService.API.Middlewares;
using Shared.Consul;
namespace PaymentService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services
                .ConfigureApiBehavior()
                .AddSwaggerWithJwt()
                .AddCustomRateLimiting()
                .AddDatabaseServices(builder.Configuration)
                .AddApplicationServices()
                .AddJwtAuthentication(builder.Configuration)
                .AddStripeConfiguration(builder.Configuration)
                .AddRabbitMqServices();
            builder.Services.AddConsul(builder.Configuration);

            var app = builder.Build();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseMiddleware<ExceptionMiddleware>();

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseAuthorization();
            app.UseRateLimiter();
            app.MapControllers();
            app.MapGet("/health", () => "Healthy");

            app.Run();
        }
    }
}
