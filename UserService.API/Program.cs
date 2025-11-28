
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.RateLimiting;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using System.Security.Claims;
using System.Text;
using System.Text.Json;
using System.Threading.RateLimiting;
using UserService.API.Extensions;
using UserService.API.Middlewares;
using UserService.API.Models.Responses;
using UserService.Application.Behaviors;
using UserService.Application.Commands.RegisterUser;
using UserService.Application.Mapping;
using UserService.Domain.Interfaces;
using UserService.Infrastructure;
using UserService.Infrastructure.Configurations;
using UserService.Infrastructure.Data;
using UserService.Infrastructure.MessagingBus;
using UserService.Infrastructure.Services;

namespace UserService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers();
            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();




            builder.Services
               .AddSwaggerWithJwt()
               .AddApplicationServices(builder.Configuration)
               .AddDatabase(builder.Configuration.GetConnectionString("DefaultConnection")!)
               .AddJwtAuthentication(builder.Configuration)
               .ConfigureApiBehavior()
               .AddCustomRateLimiting();









            //builder.Services.AddSingleton(sp => sp.GetRequiredService<IOptions<JwtSettings>>().Value);








            var app = builder.Build();

            app.UseMiddleware<ExceptionMiddleware>();

            if (app.Environment.IsDevelopment())
            {
                app.UseSwagger();
                app.UseSwaggerUI();
            }

            app.UseHttpsRedirection();
            app.UseAuthentication();
            app.UseRateLimiter();
            app.UseAuthorization();

            app.MapControllers();

            app.Run();

        }
    }
}
