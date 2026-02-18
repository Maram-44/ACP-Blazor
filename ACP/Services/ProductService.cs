using ACP.Models.Products;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class ProductService
    {
        private readonly HttpClient _httpClient;

        public ProductService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<List<Item>> GetAllProductsAsync()
        {
            // نرسل طلب GET للـ API
            var response = await _httpClient.GetFromJsonAsync<List<Item>>("api/Products");
            return response ?? new List<Item>();
        }

        public async Task<Item> CreateProductAsync(Item itemDto)
        {
            // نرسل طلب POST مع الـ DTO
            var response = await _httpClient.PostAsJsonAsync("api/Products", itemDto);

            if (response.IsSuccessStatusCode)
            {
                // إذا نجح الطلب، نقرأ المنتج المرجوع (الذي يحتوي الآن على ID)
                return await response.Content.ReadFromJsonAsync<Item>();
            }

            throw new Exception("Error creating product");
        }
    }
}
