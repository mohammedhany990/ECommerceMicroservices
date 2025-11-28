using Stripe;

namespace PaymentService.API.Extensions
{
    public static class StripeExtensions
    {
        public static IServiceCollection AddStripeConfiguration(this IServiceCollection services, IConfiguration config)
        {
            services.AddSingleton<PaymentIntentService>();

            services.AddSingleton<IStripeClient>(_ =>
            {
                var apiKey = config["Stripe:SecretKey"];

                if (string.IsNullOrEmpty(apiKey))
                    throw new InvalidOperationException("Stripe SecretKey is not configured");

                return new StripeClient(apiKey);
            });

            return services;
        }
    }
}
