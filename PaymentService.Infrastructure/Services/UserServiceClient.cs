using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Headers;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;

namespace PaymentService.Infrastructure.Services
{
    public class UserServiceClient
    {
        private readonly HttpClient _httpClient;

        public UserServiceClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }
        public async Task<string?> GetUserEmailAsync(Guid userId)
        {
            var response = await _httpClient.GetAsync($"/users/{userId}/email");
            if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                return null;

            var contentStream = await response.Content.ReadAsStreamAsync();
            var apiResponse = await JsonSerializer.DeserializeAsync<ApiResponse<string>>(
                contentStream,
                new JsonSerializerOptions { PropertyNameCaseInsensitive = true }
            );
            return apiResponse?.Data;
        }
    }
}
