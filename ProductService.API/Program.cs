using Consul;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using ProductService.API.Extensions;
using ProductService.API.Middlewares;
using Serilog;
using Shared.Consul;
using System.Text;
using System.Threading;
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
                .ConfigureApiBehavior()
                .AddConsul(builder.Configuration)
                .AddCustomHealthChecks(builder.Configuration);

            SerilogBootstrap.ConfigureSerilog(builder);

            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }
            app.UseHttpsRedirection();
            app.UseStaticFiles();
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
