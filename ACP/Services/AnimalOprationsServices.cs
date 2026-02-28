using ACP.Models.Animals;
using System.Net.Http;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class AnimalOprationsServices
    {
        private readonly HttpClient _http;

        public AnimalOprationsServices(HttpClient http)
        {
            _http = http;
        }

        // ================= Adoption =================

        public async Task<int> CreateAdoption(AdoptionDetail adoptionDetail)
        {
            var response = await _http.PostAsJsonAsync(
                "api/AnimalOprations/create-adoption-full",
                adoptionDetail
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<AdoptionDetail?> GetAdoptionDetailsAsync(int id)
        {
            return await _http.GetFromJsonAsync<AdoptionDetail>($"api/AnimalOperation/Adoption/{id}");
        }

        // ================= Foster Care =================

        public async Task<int> CreateFosterCare(FosterCare fosterCare)
        {
            var response = await _http.PostAsJsonAsync(
                "api/AnimalOprations/foster-with-animal",
                fosterCare
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<FosterCare?> GetFosterCareById(int id)
        {
            return await _http.GetFromJsonAsync<FosterCare>(
                $"api/AnimalOprations/FosterCare/{id}"
            );
        }

        // 2. الموافقة على الطلب
        public async Task<bool> ApproveAdoptionAsync(int id)
        {
            var response = await _http.PostAsync($"api/AnimalOperation/approve/{id}", null);
            return response.IsSuccessStatusCode;
        }

        // 3. رفض الطلب مع إرسال السبب
        public async Task<bool> RejectAdoptionAsync(int id, string reason)
        {
            // بما أن الـ API يتوقع string من الـ Body، نستخدم JsonContent
            var response = await _http.PostAsJsonAsync($"api/AnimalOperation/reject/{id}", reason);
            return response.IsSuccessStatusCode;
        }
    }
}
