using Microsoft.Extensions.Logging;
using OrderService.Domain.Entities;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Services
{
    public class PaymentServiceClient
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentServiceClient> _logger;

        public PaymentServiceClient(HttpClient httpClient, ILogger<PaymentServiceClient> logger)
        {
            _httpClient = httpClient;
            _logger = logger;
        }

        public async Task<PaymentResultDto?> GetPaymentStatusAsync(Guid orderId, string token, CancellationToken cancellationToken = default)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/payments/by-order/{orderId}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request, cancellationToken);

                if (!response.IsSuccessStatusCode)
                {
                    _logger.LogWarning("Failed to get payment status for OrderId {OrderId}. StatusCode: {StatusCode}", orderId, response.StatusCode);
                    return null;
                }

                var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<PaymentResultDto>>(cancellationToken: cancellationToken);
                return apiResponse?.Data;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Exception occurred while fetching payment status for OrderId {OrderId}", orderId);
                return null;
            }
        }
    }

}
