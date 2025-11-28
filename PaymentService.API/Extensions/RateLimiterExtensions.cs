using System.Security.Claims;
using System.Threading.RateLimiting;

namespace PaymentService.API.Extensions
{
    public static class RateLimiterExtensions
    {
        public static IServiceCollection AddCustomRateLimiting(this IServiceCollection services)
        {
            services.AddRateLimiter(options =>
            {
                options.AddPolicy("payment-concurrency", httpContext =>
                {
                    var userId =
                        httpContext.User.FindFirst(ClaimTypes.NameIdentifier)?.Value ??
                        "anon";

                    return RateLimitPartition.GetConcurrencyLimiter(userId, _ =>
                        new ConcurrencyLimiterOptions
                        {
                            PermitLimit = 1,
                            QueueLimit = 0,
                            QueueProcessingOrder = QueueProcessingOrder.OldestFirst
                        });
                });

                options.RejectionStatusCode = StatusCodes.Status429TooManyRequests;
            });

            return services;
        }
    }
}
