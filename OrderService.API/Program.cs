
using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using OrderService.API.Middlewares;
using OrderService.API.Models.Responses;
using OrderService.Application.Behaviors;
using OrderService.Application.Commands.CreateOrder;
using OrderService.Application.Mapping;
using OrderService.Domain.Interfaces;
using OrderService.Infrastructure.Data;
using OrderService.Infrastructure.Repositories;
using OrderService.Infrastructure.Services;
using System.Text;
using System.Text.Json;

namespace OrderService.API
{
    public class Program
    {
        public static void Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            // Add services to the container.

            builder.Services.AddControllers()
                .AddJsonOptions(options =>
                {
                    options.JsonSerializerOptions.Converters.Add(new System.Text.Json.Serialization.JsonStringEnumConverter());
                });

            // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
            builder.Services.AddEndpointsApiExplorer();
            #region Swagger
            builder.Services.AddSwaggerGen(opt =>
            {

                var securitySchema = new OpenApiSecurityScheme
                {
                    Description = "JWT Authorization header using the Bearer scheme. Example: \"Bearer {token}\"",
                    Name = "Authorization",
                    In = ParameterLocation.Header,
                    Type = SecuritySchemeType.Http,
                    Scheme = "bearer",
                    BearerFormat = "JWT",
                    Reference = new OpenApiReference
                    {
                        Type = ReferenceType.SecurityScheme,
                        Id = "Bearer"
                    }
                };

                opt.AddSecurityDefinition("Bearer", securitySchema);

                var securityRequirement = new OpenApiSecurityRequirement
                {
                            { securitySchema, new[] { "Bearer" } }
                };

                opt.AddSecurityRequirement(securityRequirement);

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Order Service", Version = "v1.0" });

            });
            #endregion


            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateOrderCommandHandler).Assembly);

            });

            builder.Services.AddValidatorsFromAssembly(typeof(CreateOrderCommandValidator).Assembly);


            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));



            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));

            builder.Services.AddAutoMapper(cfg => cfg.AddProfile<MappingProfile>(), typeof(MappingProfile).Assembly);


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));

            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value.Errors.Count > 0)
                        .SelectMany(e => e.Value.Errors)
                        .Select(m => m.ErrorMessage)
                        .ToList();

                    var response = ApiResponse<object>.FailResponse(errors, "Validation failed", 400);

                    return new BadRequestObjectResult(response);
                };
            });

            builder.Services.AddAuthentication(options =>
            {
                options.DefaultAuthenticateScheme = JwtBearerDefaults.AuthenticationScheme;
                options.DefaultChallengeScheme = JwtBearerDefaults.AuthenticationScheme;
            })
               .AddJwtBearer(options =>
               {
                   options.RequireHttpsMetadata = false;
                   options.SaveToken = true;
                   options.TokenValidationParameters = new TokenValidationParameters
                   {
                       ValidateIssuer = true,
                       ValidIssuer = builder.Configuration["JwtSettings:Issuer"],
                       ValidateAudience = true,
                       ValidAudience = builder.Configuration["JwtSettings:Audience"],
                       ValidateLifetime = true,
                       ValidateIssuerSigningKey = true,
                       IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(builder.Configuration["JwtSettings:Key"]!)),
                       ClockSkew = TimeSpan.Zero
                   };

                   // Customize 401/403 responses
                   options.Events = new JwtBearerEvents
                   {
                       OnChallenge = async context =>
                       {
                           // Prevent default 401 response
                           context.HandleResponse();

                           context.Response.StatusCode = StatusCodes.Status401Unauthorized;
                           context.Response.ContentType = "application/json";

                           var response = ApiResponse<object>.FailResponse(
                                new List<string> { "You are not authorized to access this resource." },
                                "Unauthorized",
                                StatusCodes.Status401Unauthorized
                            );

                           await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                           {
                               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                           }));
                       },
                       OnForbidden = async context =>
                       {
                           context.Response.StatusCode = StatusCodes.Status403Forbidden;
                           context.Response.ContentType = "application/json";

                           var response = ApiResponse<object>.FailResponse(
                                new List<string> { "You do not have permission to access this resource." },
                                "Forbidden",
                                StatusCodes.Status403Forbidden
                            );

                           await context.Response.WriteAsync(JsonSerializer.Serialize(response, new JsonSerializerOptions
                           {
                               PropertyNamingPolicy = JsonNamingPolicy.CamelCase
                           }));
                       }
                   };
               });

            builder.Services.AddHttpClient<ProductServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });

            builder.Services.AddHttpClient<ShippingServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });
           

            builder.Services.AddHttpClient<CartServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });

            



            var app = builder.Build();

            // Configure the HTTP request pipeline.
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
