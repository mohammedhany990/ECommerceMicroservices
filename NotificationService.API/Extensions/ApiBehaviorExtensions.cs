using NotificationService.API.Models.Responses;
using Microsoft.AspNetCore.Mvc;

namespace NotificationService.API.Extensions
{
    public static class ApiBehaviorExtensions
    {
        public static IServiceCollection ConfigureApiBehavior(this IServiceCollection services)
        {
            services.Configure<ApiBehaviorOptions>(options =>
            {
                options.SuppressModelStateInvalidFilter = true;
                options.InvalidModelStateResponseFactory = actionContext =>
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

            return services;
        }
    }
}
