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

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();

            builder.Services
               .AddSwaggerWithJwt()
               .AddApplicationServices()
               .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!)
               .AddJwtAuthentication(builder.Configuration)
               .ConfigureApiBehavior()
               .AddHttpClients()
               .AddCustomRateLimiting();

            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
