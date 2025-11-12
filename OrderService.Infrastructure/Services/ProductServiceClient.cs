using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<ProductDto?> GetProductByIdAsync(Guid Id)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<ProductDto>>($"api/products/{Id}");
                return response?.Data;
            }
            catch(Exception ex)
            {
                return null;
            }
        }
    }
}
