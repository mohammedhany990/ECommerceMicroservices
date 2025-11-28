using PaymentService.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PaymentService.Domain.Interfaces
{
    public interface IStripeWebhookService
    {
        Task<Payment?> HandleEventAsync(string json, string stripeSignatureHeader);
    }

}
