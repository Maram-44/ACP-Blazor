using ACP.Models.Products;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class OrderService
    {
        private readonly HttpClient _httpClient;

        public OrderService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. تنفيذ عملية الدفع وتحويل السلة لطلب
        public async Task<Order?> CheckoutAsync(int customerId)
        {
            // نرسل طلب POST بدون Body لأن المعرف موجود في المسار (URL)
            var response = await _httpClient.PostAsync($"api/Orders/checkout/{customerId}", null);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<Order>();
            }

            var error = await response.Content.ReadAsStringAsync();
            throw new Exception(error); // أو يمكنك التعامل مع الخطأ حسب رغبتك
        }

        // 2. جلب قائمة طلبات العميل
        public async Task<List<Order>> GetMyOrdersAsync(int customerId)
        {
            var response = await _httpClient.GetAsync($"api/Orders/my-orders/{customerId}");

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<List<Order>>() ?? new List<Order>();
            }

            return new List<Order>();
        }

        // 3. جلب تفاصيل طلب معين برقم الطلب
        public async Task<Order?> GetOrderByIdAsync(int orderId)
        {
            return await _httpClient.GetFromJsonAsync<Order>($"api/Orders/{orderId}");
        }
    }
}
