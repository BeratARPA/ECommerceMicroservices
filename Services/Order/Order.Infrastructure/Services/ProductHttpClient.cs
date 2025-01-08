using Order.Domain.DTOs;
using Order.Infrastructure.Interfaces;
using System.Net.Http.Json;

namespace Order.Infrastructure.Services
{
    public class ProductHttpClient : IProductHttpClient
    {
        private readonly HttpClient _httpClient;

        public ProductHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _httpClient.BaseAddress = new Uri("http://productapi:9001/"); // Product Service URL
        }

        public async Task<ProductDTO?> GetProductAsync(int productId)
        {
            try
            {
                return await _httpClient.GetFromJsonAsync<ProductDTO>($"api/product/{productId}");
            }
            catch
            {
                return null;
            }
        }

        public async Task<bool> UpdateProductStockAsync(int productId, int newStock)
        {
            try
            {
                var response = await _httpClient.PutAsJsonAsync($"api/product/{productId}", new { Stock = newStock });
                return response.IsSuccessStatusCode;
            }
            catch
            {
                return false;
            }
        }
    }
}
