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
    public class ShippingServiceClient
    {
        private readonly HttpClient _httpClient;

        public ShippingServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ShippingCostResultDto?> CalculateShippingCostAsync(ShippingCostRequestDto dto, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "/ShippingMethods/calculate")
                {
                    Content = JsonContent.Create(dto)
                };

                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ShippingCostResultDto>>();
                return result?.Data;
            }
            catch
            {
                return null;
            }
        }

        public async Task<ShippingMethodDto?> GetShippingMethodByIdAsync(Guid id, string token)
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Get, $"/ShippingMethods/{id}");
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);

                var response = await _httpClient.SendAsync(request);
                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ShippingMethodDto>>();
                return result?.Data;
            }
            catch(Exception ex)
            {
                throw new Exception($"Failed to get shipping method {id}. Error: {ex.Message}", ex);

            }
        }


    }
}
