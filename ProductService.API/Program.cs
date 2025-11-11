using FluentValidation;
using MediatR;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
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

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "Product Service", Version = "v1.0" });

            });
            #endregion


            builder.Services.AddHttpClient<CategoryServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5255/");
            });



            builder.Services.AddAutoMapper(cfg =>
            {
                cfg.AddProfile<MappingProfile>();
            }, typeof(MappingProfile).Assembly);


            builder.Services.AddScoped(typeof(IRepository<>), typeof(Repository<>));


            builder.Services.AddDbContext<AppDbContext>(options =>
                options.UseNpgsql(builder.Configuration.GetConnectionString("DefaultConnection")));


            builder.Services.AddScoped<IFileService, FileService>();


            builder.Services.AddMediatR(cfg =>
            {
                cfg.RegisterServicesFromAssembly(typeof(CreateProductCommandHandler).Assembly);

            });

            builder.Services.AddValidatorsFromAssembly(typeof(CreateProductCommandValidator).Assembly);


            builder.Services.AddTransient(typeof(IPipelineBehavior<,>), typeof(ValidationBehavior<,>));


            builder.Services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;

                options.InvalidModelStateResponseFactory = (actionContext) =>
                {
                    var errors = actionContext.ModelState
                        .Where(e => e.Value?.Errors.Count > 0)
                        .SelectMany(e => e.Value!.Errors)
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
