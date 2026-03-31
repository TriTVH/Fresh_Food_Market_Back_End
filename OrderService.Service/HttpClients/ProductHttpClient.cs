using OrderService.Service.DTO;
using OrderService.Service.DTO.External;
using System.Net.Http.Json;

namespace OrderService.Service.HttpClients
{
    public class ProductHttpClient : IProductHttpClient
    {
        private readonly HttpClient _httpClient;

        public ProductHttpClient(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<ProductDTO?> GetProductByIdAsync(int productId)
        {
            var response = await _httpClient.GetAsync($"api/product/{productId}");
            if (!response.IsSuccessStatusCode)
                return null;

            var apiResponse = await response.Content.ReadFromJsonAsync<ApiResponse<ProductDTO>>();
            return apiResponse?.Data;
        }
    }
}
