using System.Security.Claims;
using System.Threading.RateLimiting;

namespace OrderService.API.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("order-per-user", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst("sub")?.Value ??
                        httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                        "anonymous";

                    return RateLimitPartition.GetSlidingWindowLimiter(userId, _ =>
                        new SlidingWindowRateLimiterOptions
                        {
                            PermitLimit = 10,
                            Window = TimeSpan.FromMinutes(1),
                            SegmentsPerWindow = 4,
                            QueueLimit = 0
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
