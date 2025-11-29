using Microsoft.EntityFrameworkCore;
using OrderService.API.Extensions;
using OrderService.API.Middlewares;

namespace OrderService.API
{
    public class Program
    {
        public static void Main(string[] args)
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
               .AddRabbitMqServices();

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

            app.Run();
        }
    }
}
