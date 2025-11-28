using Microsoft.AspNetCore.RateLimiting;
using System.Security.Claims;
using System.Threading.RateLimiting;

namespace UserService.API.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                // Global limit
                options.AddFixedWindowLimiter("global", opts =>
                {
                    opts.PermitLimit = 100;
                    opts.Window = TimeSpan.FromMinutes(1);
                    opts.QueueLimit = 0;
                });

                // Login per IP
                options.AddFixedWindowLimiter("login-per-ip", opts =>
                {
                    opts.PermitLimit = 5;
                    opts.Window = TimeSpan.FromMinutes(1);
                    opts.QueueLimit = 0;
                });

                // Register per IP
                options.AddFixedWindowLimiter("register-per-ip", opts =>
                {
                    opts.PermitLimit = 3;
                    opts.Window = TimeSpan.FromMinutes(1);
                    opts.QueueLimit = 0;
                });

                // Refresh token abuse protection – Per User
                options.AddPolicy("per-user", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    return RateLimitPartition.GetTokenBucketLimiter(userId, _ =>
                        new TokenBucketRateLimiterOptions
                        {
                            TokenLimit = 20,
                            TokensPerPeriod = 20,
                            ReplenishmentPeriod = TimeSpan.FromMinutes(1),
                            AutoReplenishment = true,
                            QueueLimit = 0
                        });
                });

                // Slow-down policy (delays requests instead of blocking)
                options.AddPolicy("slow", httpContext =>
                {
                    return RateLimitPartition.GetSlidingWindowLimiter("slow-down", _ =>
                        new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 50,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 3,
                            QueueLimit = 0
                        });
                });

                // Default rejection status code
                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
