using ACP.Models.Customers;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class CustomerService
    {
        private readonly HttpClient _http;

        public CustomerService(HttpClient http) => _http = http;

        // 1. جلب بيانات الملف الشخصي
        public async Task<CustomerProfile?> GetProfileAsync()
        {
            var response = await _http.GetAsync("api/Customers/profile");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CustomerProfile>()
                : null;
        }

        // 2. تحديث بيانات الملف الشخصي (ترجع رسالة الخطأ أو "Success")
        public async Task<string> UpdateProfileAsync(CustomerProfile dto)
        {
            var response = await _http.PutAsJsonAsync("api/Customers/update-profile", dto);

            if (response.IsSuccessStatusCode)
                return "Success";

            // قراءة رسالة الخطأ القادمة من الـ API (BadRequest)
            return await response.Content.ReadAsStringAsync();
        }
    }
}
