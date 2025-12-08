using Microsoft.EntityFrameworkCore;
using ShippingService.API.Extensions;
using ShippingService.API.Middlewares;
using Shared.Consul;
namespace ShippingService.API
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
               .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!)
               .AddJwtAuthentication(builder.Configuration)
               .ConfigureApiBehavior();

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

            app.MapControllers();

            app.Run();
        }
    }
}
