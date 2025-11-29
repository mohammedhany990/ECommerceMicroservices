using CategoryService.API.Extensions;
using CategoryService.API.Middlewares;
using Microsoft.EntityFrameworkCore;
using ProductService.API.Extensions;

namespace CategoryService.API
{
    public class Program
    {
        public static void Main(string[] args)
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
