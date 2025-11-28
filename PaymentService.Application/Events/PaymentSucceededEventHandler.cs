using MediatR;
using Microsoft.Extensions.Logging;
using PaymentService.Application.Events;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Threading;
using System.Threading.Tasks;

namespace PaymentService.Application.Events
{
    public class PaymentSucceededEventHandler : INotificationHandler<PaymentSucceededEvent>
    {
        private readonly HttpClient _httpClient;
        private readonly ILogger<PaymentSucceededEventHandler> _logger;
        private readonly string _orderServiceToken;

        public PaymentSucceededEventHandler(HttpClient httpClient, ILogger<PaymentSucceededEventHandler> logger, string orderServiceToken = "")
        {
            _httpClient = httpClient;
            _logger = logger;
            _orderServiceToken = orderServiceToken;
        }

        public async Task Handle(PaymentSucceededEvent notification, CancellationToken cancellationToken)
        {
            try
            {
                var updateRequest = new
                {
                    OrderId = notification.OrderId,
                    Status = "Paid" 
                };

                if (!string.IsNullOrWhiteSpace(_orderServiceToken))
                {
                    _httpClient.DefaultRequestHeaders.Authorization =
                        new AuthenticationHeaderValue("Bearer", _orderServiceToken);
                }

                var response = await _httpClient.PutAsJsonAsync(
                    $"/orders/{notification.OrderId}",
                    updateRequest,
                    cancellationToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogInformation("Order {OrderId} updated to Paid successfully.", notification.OrderId);
                }
                else
                {
                    var content = await response.Content.ReadAsStringAsync(cancellationToken);
                    _logger.LogWarning("Failed to update Order {OrderId}. Status: {Status}, Response: {Content}",
                        notification.OrderId, response.StatusCode, content);
                }
            }
            catch (HttpRequestException ex)
            {
                _logger.LogError(ex, "HTTP error while updating Order {OrderId}", notification.OrderId);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Unexpected error while updating Order {OrderId}", notification.OrderId);
            }
        }
    }
}
