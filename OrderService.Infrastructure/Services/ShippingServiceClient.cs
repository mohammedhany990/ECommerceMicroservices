using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
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
        public async Task<ShippingCostResultDto?> CalculateShippingCostAsync(ShippingCostRequestDto request)
        {
            try
            {
                var response = await _httpClient.PostAsJsonAsync("/ShippingMethods/calculate", request);

                response.EnsureSuccessStatusCode();

                var result = await response.Content.ReadFromJsonAsync<ApiResponse<ShippingCostResultDto>>();
                return result?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }

        public async Task<ShippingMethodDto?> GetShippingMethodByIdAsync(Guid id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<ShippingMethodDto>>($"api/ShippingMethods/{id}");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }


    }
}
