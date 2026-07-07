using ACP.Features.MedicalCenters.DTOs;
using System.Net.Http.Json;

namespace ACP.Features.MedicalCenters
{
    public class MedicalCenterService
    {
        private readonly HttpClient _http;

        public MedicalCenterService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MedicalCenter>> GetAll()
        {
            return await _http.GetFromJsonAsync<List<MedicalCenter>>(
                "api/MedicalCenters"
            ) ?? new List<MedicalCenter>();
        }

        public async Task<MedicalCenter?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<MedicalCenter>(
                $"api/MedicalCenters/{id}"
            );
        }

        public async Task<int> Create(MedicalCenter medicalCenter)
        {
            var response = await _http.PostAsJsonAsync(
                "api/MedicalCenters",
                medicalCenter
            );

            response.EnsureSuccessStatusCode();

            var result = await response.Content
                .ReadFromJsonAsync<Dictionary<string, int>>();

            return result!["Id"];
        }

        public async Task<bool> Update(int id, MedicalCenter medicalCenter)
        {
            var response = await _http.PutAsJsonAsync(
                $"api/MedicalCenters/{id}",
                medicalCenter
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _http.DeleteAsync(
                $"api/MedicalCenters/{id}"
            );

            return response.IsSuccessStatusCode;
        }
    }
}
