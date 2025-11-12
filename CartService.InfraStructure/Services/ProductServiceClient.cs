using CartService.Domain.Interfaces;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace CartService.InfraStructure.Services
{
    public class ProductServiceClient
    {
        private readonly HttpClient _httpClient;

        public ProductServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDto?> GetProductByIdAsync(Guid productId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<ProductDto>>($"/products/{productId}");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }
}
