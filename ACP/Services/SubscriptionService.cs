using System.Net.Http.Json;
using ACP.Models.Customers;

namespace ACP.Services
{
    public class SubscriptionService
    {
        private readonly HttpClient _http;
        public SubscriptionService(HttpClient http) => _http = http;

        public async Task<List<Subscription>> GetAllAsync() =>
            await _http.GetFromJsonAsync<List<Subscription>>("api/Subscriptions") ?? new();

        public async Task<Subscription?> GetByIdAsync(int id) =>
            await _http.GetFromJsonAsync<Subscription>($"api/Subscriptions/{id}");

        public async Task<bool> CreateAsync(Subscription dto) =>
            (await _http.PostAsJsonAsync("api/Subscriptions", dto)).IsSuccessStatusCode;

        public async Task<bool> UpdateAsync(Subscription dto) =>
            (await _http.PutAsJsonAsync("api/Subscriptions", dto)).IsSuccessStatusCode;

        public async Task<bool> DeleteAsync(int id) =>
            (await _http.DeleteAsync($"api/Subscriptions/{id}")).IsSuccessStatusCode;


        public async Task<bool> ActivateSubscriptionAsync(CustomerSubscription dto)
        {
            var response = await _http.PostAsJsonAsync("api/CustomerSubscription/activate", dto);
            return response.IsSuccessStatusCode;
        }

        public async Task<int> GetRemainingDaysAsync(int customerId)
        {
            return await _http.GetFromJsonAsync<int>($"api/CustomerSubscription/remaining-days/{customerId}");
        }
    }
}
