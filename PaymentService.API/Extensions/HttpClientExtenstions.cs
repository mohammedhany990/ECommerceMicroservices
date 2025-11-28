
using PaymentService.Application.Events;
using PaymentService.Infrastructure.Services;

namespace PaymentService.API.Extensions
{
    public static class HttpClientExtenstions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<OrderServiceClient>(c =>
            c.BaseAddress = new Uri("http://localhost:5240/"));

            services.AddHttpClient<PaymentSucceededEventHandler>(c =>
                c.BaseAddress = new Uri("http://localhost:5240/"));

            services.AddHttpClient<UserServiceClient>(c =>
                c.BaseAddress = new Uri("http://localhost:5240/"));

            return services;
        }
    }
}
