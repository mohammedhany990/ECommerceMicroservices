
using CartService.API.Extensions;
using CartService.API.Middlewares;
using CartService.API.Models.Responses;
using CartService.Application.Behaviors;
using CartService.Application.Commands.AddItemToCart;
using CartService.Application.Mapping;
using CartService.Domain.Interfaces;
using CartService.Infrastructure.MessageBus;
using CartService.InfraStructure.MessageBus;
using CartService.InfraStructure.Repositories;
using CartService.InfraStructure.Services;
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using StackExchange.Redis;
using System.Text;
using System.Text.Json;

namespace CartService.API
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
                .AddApplicationServices()
                .AddDatabase(builder.Configuration)
                .AddJwtAuthentication(builder.Configuration)
                .ConfigureApiBehavior()
                .AddHttpClients();


            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
