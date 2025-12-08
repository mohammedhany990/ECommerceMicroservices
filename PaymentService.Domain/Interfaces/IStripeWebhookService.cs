using PaymentService.Domain.Entities;

namespace PaymentService.Domain.Interfaces
{
    public interface IStripeWebhookService
    {
        Task<Payment?> HandleEventAsync(string json, string stripeSignatureHeader);
    }

}
