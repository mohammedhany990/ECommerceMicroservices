using Microsoft.EntityFrameworkCore;
using NotificationService.API.Extensions;
using NotificationService.API.Middlewares;
using Shared.Consul;
namespace NotificationService.API
{
    public class Program
    {
        public static async Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);


            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();

            builder.Services
               .AddSwaggerWithJwt()
               .AddApplicationServices()
               .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection"))
               .AddJwtAuthentication(builder.Configuration)
               .ConfigureApiBehavior()
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
