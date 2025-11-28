using Shared.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Text;
using System.Threading.Tasks;

namespace OrderService.Infrastructure.Services
{
    //public class CartServiceClient
    //{
    //    private readonly HttpClient _httpClient;

    //    public CartServiceClient(HttpClient httpClient)
    //    {
    //        _httpClient = httpClient;
    //    }

    //    public async Task<CartDto?> GetCartForUser(string token)
    //    {
    //        try
    //        {
    //            var request = new HttpRequestMessage(HttpMethod.Get, $"/carts/");
    //            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    //            var response = await _httpClient.SendAsync(request);
    //            response.EnsureSuccessStatusCode();

    //            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    //            return apiResponse?.Data;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }

    //    public async Task<bool> ClearCartForUser(string token)
    //    {
    //        try
    //        {
    //            var request = new HttpRequestMessage(HttpMethod.Delete, "/carts/clear");
    //            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    //            var response = await _httpClient.SendAsync(request);
    //            response.EnsureSuccessStatusCode();

    //            return true;
    //        }
    //        catch
    //        {
    //            return false;
    //        }
    //    }

    //    public async Task<CartDto?> RestoreItemsToCart(string token, List<CartItemDto> items)
    //    {
    //        try
    //        {
    //            var request = new HttpRequestMessage(HttpMethod.Post, "/carts/restore")
    //            {
    //                Content = JsonContent.Create(items)
    //            };
    //            request.Headers.Authorization = new System.Net.Http.Headers.AuthenticationHeaderValue("Bearer", token);

    //            var response = await _httpClient.SendAsync(request);
    //            response.EnsureSuccessStatusCode();

    //            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<CartDto>>();
    //            return apiResponse?.Data;
    //        }
    //        catch
    //        {
    //            return null;
    //        }
    //    }


    //}
}
