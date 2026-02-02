using System.Net.Http.Json;
using ACP.Models.MedicalCenters;

namespace ACP.Services
{
    public class MedicalCenterReservationService
    {
        private readonly HttpClient _http;

        public MedicalCenterReservationService(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<MedicalCenterReservation>> GetAll()
        {
            return await _http.GetFromJsonAsync<List<MedicalCenterReservation>>(
                "api/MedicalCenterReservation"
            ) ?? new List<MedicalCenterReservation>();
        }

        public async Task<List<MedicalCenterReservation>> GetAllByCustomerId(int customerId)
        {
            return await _http.GetFromJsonAsync<List<MedicalCenterReservation>>(
                $"api/MedicalCenterReservation/by-customer/{customerId}"
            ) ?? new List<MedicalCenterReservation>();
        }

        public async Task<MedicalCenterReservation?> GetById(int id)
        {
            return await _http.GetFromJsonAsync<MedicalCenterReservation>(
                $"api/MedicalCenterReservation/{id}"
            );
        }

        public async Task<int> Create(MedicalCenterReservation Reservation)
        {
            var response = await _http.PostAsJsonAsync(
                "api/MedicalCenterReservation",
                Reservation
            );

            response.EnsureSuccessStatusCode();

            return await response.Content.ReadFromJsonAsync<int>();
        }

        public async Task<bool> Update(int id, MedicalCenterReservation Reservation)
        {
            var response = await _http.PutAsJsonAsync(
                $"api/MedicalCenterReservation/{id}",
                Reservation
            );

            return response.IsSuccessStatusCode;
        }

        public async Task<bool> Delete(int id)
        {
            var response = await _http.DeleteAsync(
                $"api/MedicalCenterReservation/{id}"
            );

            return response.IsSuccessStatusCode;
        }
    }
}
