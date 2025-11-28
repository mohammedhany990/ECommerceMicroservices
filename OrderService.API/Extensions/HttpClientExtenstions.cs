
using OrderService.Infrastructure.Services;

namespace OrderService.API.Extensions
{
    public static class HttpClientExtenstions
    {
        public static IServiceCollection AddHttpClients(this IServiceCollection services)
        {
            services.AddHttpClient<ProductServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });

            services.AddHttpClient<ShippingServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });

            services.AddHttpClient<UserServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });



            services.AddHttpClient<PaymentServiceClient>(client =>
            {
                client.BaseAddress = new Uri("http://localhost:5240/");
            });

            return services;
        }
    }
}
