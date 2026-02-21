using ACP.Application.DTOs.Products;
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

        public async Task<Item?>GetProductByIdAsync(int itemId)
        {
            return await _httpClient.GetFromJsonAsync<Item>($"api/Products/{itemId}");
        }

        // 1. إرسال تعليق جديد
        public async Task<ProductComment> AddCommentAsync(ProductComment commentDto)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Products/comments", commentDto);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<ProductComment>();
            }

            throw new Exception("فشل في إضافة التعليق");
        }

        // 2. جلب تعليقات منتج معين
        public async Task<List<ProductComment>> GetProductCommentsAsync(int itemId)
        {
            // نستخدم المسار الجديد الذي حددناه في الكنترولر
            var response = await _httpClient.GetFromJsonAsync<List<ProductComment>>($"api/Products/{itemId}/comments");
            return response ?? new List<ProductComment>();
        }

        // 3. حذف تعليق
        public async Task<bool> DeleteCommentAsync(int commentId, int customerId)
        {
            var response = await _httpClient.DeleteAsync($"api/Products/comments/{commentId}/{customerId}");
            return response.IsSuccessStatusCode;
        }
    }
}
