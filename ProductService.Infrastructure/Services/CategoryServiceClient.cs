using ProductService.Domain.Entities;
using ProductService.Domain.Interfaces;
using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace ProductService.Infrastructure.Services
{
    public class CategoryServiceClient
    {
        private readonly HttpClient _httpClient;

        public CategoryServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<CategoryDto>> GetAllCategoriesAsync()
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<List<CategoryDto>>>("api/categories");

                return response?.Data ?? new List<CategoryDto>();
            }
            catch (Exception)
            {
                return new List<CategoryDto>();
            }
        }

        public async Task<CategoryDto?> GetCategoryByIdAsync(Guid categoryId)
        {
            try
            {
                var response = await _httpClient.GetFromJsonAsync<ApiResponse<CategoryDto>>($"api/categories/{categoryId}");
                return response?.Data;
            }
            catch (Exception)
            {
                return null;
            }
        }
    }

}
