using Microsoft.EntityFrameworkCore;
using ProductService.API.Extensions;
using ProductService.API.Middlewares;
using ProductService.Infrastructure.Services;

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
            app.Run();
        }
    }
}
