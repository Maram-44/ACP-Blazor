using ACP.Models.Products;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class BasketService
    {
        private readonly HttpClient _httpClient;

        public BasketService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        public async Task<bool> AddToBasketAsync(ItemsBasket basketDto)
        {
            // نرسل طلب POST إلى api/Basket/add
            var response = await _httpClient.PostAsJsonAsync("api/Basket/add", basketDto);

            // نرجع true إذا كان الرد 200 Ok
            return response.IsSuccessStatusCode;
        }

        public async Task<List<ItemsBasket>> GetCustomerBasketAsync(int customerId)
        {
            // نرسل طلب GET مع تمرير الـ customerId في الرابط
            var response = await _httpClient.GetFromJsonAsync<List<ItemsBasket>>($"api/Basket/{customerId}");
            return response ?? new List<ItemsBasket>();
        }
    }
}
