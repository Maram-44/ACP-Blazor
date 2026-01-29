using System.Net.Http.Json;
using ACP.Models.Animals;

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
                "api/AnimalOprations/Adoption",
                adoptionDetail
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<AdoptionDetail?> GetAdoptionById(int id)
        {
            return await _http.GetFromJsonAsync<AdoptionDetail>(
                $"api/AnimalOprations/Adoption/{id}"
            );
        }

        // ================= Foster Care =================

        public async Task<int> CreateFosterCare(FosterCare fosterCare)
        {
            var response = await _http.PostAsJsonAsync(
                "api/AnimalOprations/FosterCare",
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
    }
}
