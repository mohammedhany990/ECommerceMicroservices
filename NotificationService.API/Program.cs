using Microsoft.EntityFrameworkCore;
using NotificationService.API.Extensions;
using NotificationService.API.Middlewares;

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


            app.MapControllers();

            app.Run();
        }
    }
}
