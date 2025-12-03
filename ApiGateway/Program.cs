using Microsoft.OpenApi.Models;
using Ocelot.DependencyInjection;
using Ocelot.Middleware;

namespace ApiGateway
{
    public class Program
    {
        public async static Task Main(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);

            builder.Configuration
                .AddJsonFile("ocelot.json", optional: false, reloadOnChange: true)
                .AddJsonFile("ocelot.SwaggerEndPoints.json", optional: false, reloadOnChange: true);

            builder.Services.AddSwaggerForOcelot(builder.Configuration);

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
                opt.AddSecurityRequirement(new OpenApiSecurityRequirement
                {
                    { securitySchema, Array.Empty<string>() }
                });

                opt.SwaggerDoc("v1", new OpenApiInfo { Title = "API Gateway", Version = "v1.0" });
            });


            builder.Services.AddOcelot(builder.Configuration);




            var app = builder.Build();

            app.UseAuthentication();
            app.UseAuthorization();
            app.UseStaticFiles();

            app.UseSwaggerForOcelotUI(opt =>
            {
                opt.PathToSwaggerGenerator = "/swagger/docs";
            });

            await app.UseOcelot();

            app.Run();
        }
    }
}
