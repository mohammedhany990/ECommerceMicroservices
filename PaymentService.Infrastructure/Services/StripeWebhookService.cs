using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using PaymentService.Domain.Entities;
using PaymentService.Domain.Interfaces;
using Shared.Enums;
using Stripe;

namespace PaymentService.Infrastructure.Services
{
    public class StripeWebhookService : IStripeWebhookService
    {
        private readonly IPaymentRepository _paymentRepository;
        private readonly string _webhookSecret;
        private readonly ILogger<StripeWebhookService> _logger;

        public StripeWebhookService(
            IPaymentRepository paymentRepository,
            IConfiguration configuration,
            ILogger<StripeWebhookService> logger)
        {
            _paymentRepository = paymentRepository;
            _webhookSecret = configuration["Stripe:WebhookSecret"];
            _logger = logger;
        }

        public async Task<Payment?> HandleEventAsync(string json, string stripeSignatureHeader)
        {
            Event stripeEvent;

            try
            {
                stripeEvent = EventUtility.ConstructEvent(
                    json,
                    stripeSignatureHeader,
                    _webhookSecret,
                    throwOnApiVersionMismatch: false);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Invalid Stripe webhook signature.");
                throw new Exception("Invalid Stripe webhook signature.");
            }

            switch (stripeEvent.Type)
            {
                case "payment_intent.succeeded":
                    {
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        return await UpdatePaymentStatusAsync(intent, PaymentStatus.Succeeded);
                    }

                case "payment_intent.payment_failed":
                    {
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        await UpdatePaymentStatusAsync(intent, PaymentStatus.Failed, intent?.LastPaymentError?.Message);
                        return null;
                    }

                case "payment_intent.canceled":
                    {
                        var intent = stripeEvent.Data.Object as PaymentIntent;
                        await UpdatePaymentStatusAsync(intent, PaymentStatus.Canceled, "Canceled by Stripe");
                        return null;
                    }

                default:
                    _logger.LogInformation($"Unhandled Stripe event type: {stripeEvent.Type}");
                    return null;
            }
        }

        private async Task<Payment?> UpdatePaymentStatusAsync(
            PaymentIntent? intent,
            PaymentStatus status,
            string? failureReason = null)
        {
            if (intent == null)
            {
                _logger.LogWarning("Stripe PaymentIntent object is null.");
                return null;
            }

            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(intent.Id);

            if (payment == null)
            {
                _logger.LogWarning($"Payment record not found for PaymentIntentId: {intent.Id}");
                return null;
            }

            payment.Status = status;
            payment.FailureReason = failureReason;

            if (status == PaymentStatus.Succeeded)
                payment.ConfirmedAt = DateTime.UtcNow;

            if (status == PaymentStatus.Canceled)
                payment.CanceledAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation($"Payment {payment.Id} status updated to {status}");

            return payment;
        }
    }
}
