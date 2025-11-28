using PaymentService.API.Extensions;
using PaymentService.API.Middlewares;
namespace PaymentService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args)

            builder.Services.AddControllers();

            builder.Services.AddEndpointsApiExplorer();

            builder.Services
                .ConfigureApiBehavior()
                .AddSwaggerWithJwt()
                .AddCustomRateLimiting()
                .AddDatabaseServices(builder.Configuration)
                .AddApplicationServices()
                .AddRabbitMqServices()
                .AddJwtAuthentication(builder.Configuration)
                .AddStripeConfiguration(builder.Configuration)
                .AddHttpClients();
       
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
