using CartService.InfraStructure.Services;

namespace CartService.API.Extensions
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


            return services;
        }
    }
}
