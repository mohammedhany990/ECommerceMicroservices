using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using ProductService.API.Extensions;
using ProductService.API.Middlewares;
using ProductService.API.Models.Responses;
using ProductService.Application.Behaviors;
using ProductService.Application.Commands.CreateProduct;
using ProductService.Application.Mapping;
using ProductService.Domain.Interfaces;
using ProductService.Infrastructure.Data;
using ProductService.Infrastructure.Repository;
using ProductService.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace ProductService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Services.AddControllers();
            builder.Services.AddEndpointsApiExplorer();


            builder.Services.AddHttpClient<CategoryServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5255/");
            });


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
