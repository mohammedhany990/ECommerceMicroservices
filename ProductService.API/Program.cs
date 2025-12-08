using Consul;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductService.API.Extensions;
using ProductService.API.Middlewares;
using System.Text;
using System.Threading;
using Shared.Consul;
namespace ProductService.API
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

            // Configure the HTTP request pipeline.
            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseMiddleware<ExceptionMiddleware>();
            app.UseHttpsRedirection();
            app.UseStaticFiles();
            app.UseAuthentication();
            app.UseAuthorization();
            app.MapControllers();
            app.MapGet("/health", () => "Healthy");

            //app.RegisterConsul(serviceSettings);

            app.Run();
        }
    }
}
