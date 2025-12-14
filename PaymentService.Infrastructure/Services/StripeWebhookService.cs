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
            _webhookSecret = configuration["Stripe:WebhookSecret"]
                ?? throw new ArgumentNullException("Stripe:WebhookSecret not configured");
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
            catch (StripeException ex)
            {
                _logger.LogError(ex, "Invalid Stripe webhook signature.");
                return null;
            }

            if (stripeEvent.Data.Object is not PaymentIntent intent)
            {
                _logger.LogWarning("Stripe event does not contain PaymentIntent. EventType: {EventType}", stripeEvent.Type);
                return null;
            }

            _logger.LogInformation(
                "Stripe webhook received. EventType: {EventType}, PaymentIntentId: {PaymentIntentId}",
                stripeEvent.Type,
                intent.Id);

            return stripeEvent.Type switch
            {
                "payment_intent.succeeded" => await HandleSucceededAsync(intent),
                "payment_intent.payment_failed" => await HandleFailedAsync(intent),
                "payment_intent.canceled" => await HandleCanceledAsync(intent),
                _ => HandleUnhandledEvent(stripeEvent.Type)
            };

        }

        //PRIVATE HANDLERS
        private async Task<Payment?> HandleSucceededAsync(PaymentIntent intent)
        {
            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(intent.Id);
            if (payment == null)
            {
                _logger.LogWarning("Payment not found for PaymentIntentId {PaymentIntentId}", intent.Id);
                return null;
            }

            // IDEMPOTENCY CHECK (MOST IMPORTANT LINE)
            if (payment.Status == PaymentStatus.Paid)
            {
                _logger.LogInformation(
                    "Payment {PaymentId} already marked as Paid. Skipping duplicate webhook.",
                    payment.Id);
                return null;
            }

            payment.Status = PaymentStatus.Paid;
            payment.ConfirmedAt = DateTime.UtcNow;
            payment.FailureReason = null;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation(
                "Payment {PaymentId} successfully marked as Paid",
                payment.Id);

            return payment;
        }

        private async Task<Payment?> HandleFailedAsync(PaymentIntent intent)
        {
            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(intent.Id);
            if (payment == null)
                return null;

            if (payment.Status == PaymentStatus.Paid)
            {
                _logger.LogWarning(
                    "Ignoring failed event for already Paid payment {PaymentId}",
                    payment.Id);
                return null;
            }

            payment.Status = PaymentStatus.Failed;
            payment.FailureReason = intent.LastPaymentError?.Message;
            payment.UpdatedAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} marked as Failed", payment.Id);
            return null;
        }

        private async Task<Payment?> HandleCanceledAsync(PaymentIntent intent)
        {
            var payment = await _paymentRepository.GetByPaymentIntentIdAsync(intent.Id);
            if (payment == null)
                return null;

            if (payment.Status == PaymentStatus.Paid)
            {
                _logger.LogWarning(
                    "Ignoring canceled event for already Paid payment {PaymentId}",
                    payment.Id);
                return null;
            }

            payment.Status = PaymentStatus.Canceled;
            payment.CanceledAt = DateTime.UtcNow;

            await _paymentRepository.UpdateAsync(payment);
            await _paymentRepository.SaveChangesAsync();

            _logger.LogInformation("Payment {PaymentId} marked as Canceled", payment.Id);
            return null;
        }

        private Payment? HandleUnhandledEvent(string eventType)
        {
            _logger.LogInformation("Unhandled Stripe event type: {EventType}", eventType);
            return null;
        }
    }
}
