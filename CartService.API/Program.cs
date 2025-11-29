using CartService.API.Extensions;
using CartService.API.Middlewares;

namespace CartService.API
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
                .AddDatabase(builder.Configuration)
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
            app.UseAuthorization();
            app.MapControllers();

            app.Run();
        }
    }
}
