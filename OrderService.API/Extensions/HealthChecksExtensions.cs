using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Diagnostics.HealthChecks;
using System.Text.Json;
namespace OrderService.API.Extensions
{
    public static class HealthChecksExtensions
    {
        public static IServiceCollection AddCustomHealthChecks(this IServiceCollection services, IConfiguration configuration)
        {
            var hcBuilder = services.AddHealthChecks();

            // PostgreSQL
            var pgConn = configuration.GetConnectionString("DefaultConnection");
            if (!string.IsNullOrEmpty(pgConn))
            {
                hcBuilder.AddNpgSql(
                    pgConn,
                    name: "postgres",
                    tags: new[] { "db", "ready" },
                    timeout: TimeSpan.FromSeconds(5)
                );
            }

            // Redis
            var redisConn = configuration.GetConnectionString("Redis");
            if (!string.IsNullOrEmpty(redisConn))
            {
                hcBuilder.AddRedis(
                    redisConn,
                    name: "redis",
                    tags: new[] { "cache", "ready" },
                    timeout: TimeSpan.FromSeconds(5)
                );
            }

            // RabbitMQ
            var rabbitHost = configuration["RabbitMQ:Host"];
            var rabbitUser = configuration["RabbitMQ:Username"] ?? "guest";
            var rabbitPass = configuration["RabbitMQ:Password"] ?? "guest";

            if (!string.IsNullOrEmpty(rabbitHost))
            {
                hcBuilder.AddUrlGroup(
                    new Uri($"http://{rabbitHost}:15672/"),
                    name: "rabbitmq-mgmt",
                    tags: new[] { "mq", "ready" }
                );
            }

            // Self check
            hcBuilder.AddCheck("self", () => HealthCheckResult.Healthy(), tags: new[] { "live" });

            return services;
        }


        public static IApplicationBuilder UseHealthChecksEndpoints(this IApplicationBuilder app)
        {
            app.UseEndpoints(endpoints =>
            {
                // Liveness
                endpoints.MapHealthChecks("/health/live", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("live"),
                    ResponseWriter = WriteResponse
                });

                // Readiness
                endpoints.MapHealthChecks("/health/ready", new Microsoft.AspNetCore.Diagnostics.HealthChecks.HealthCheckOptions
                {
                    Predicate = check => check.Tags.Contains("ready"),
                    ResponseWriter = WriteResponse
                });
            });

            return app;
        }

        private static Task WriteResponse(Microsoft.AspNetCore.Http.HttpContext context, HealthReport report)
        {
            context.Response.ContentType = "application/json";

            var result = JsonSerializer.Serialize(new
            {
                status = report.Status.ToString(),
                totalDuration = report.TotalDuration.TotalMilliseconds,
                checks = report.Entries.Select(entry => new
                {
                    name = entry.Key,
                    status = entry.Value.Status.ToString(),
                    description = entry.Value.Description,
                    duration = entry.Value.Duration.TotalMilliseconds,
                    exception = entry.Value.Exception?.Message
                })
            });

            return context.Response.WriteAsync(result);
        }
    }
}
