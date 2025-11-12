using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Services
{
    public class CartServiceClient
    {
        private readonly HttpClient _httpClient;

        public CartServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<CartDto?> GetCartForUser()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<CartDto>>($"api/carts");
                return response?.Data;
            }
            catch (Exception ex)
            {
                return null;
            }
        }

    }
}
