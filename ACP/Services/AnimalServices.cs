using System.Net.Http.Json;
using ACP.Models.Animals;

namespace ACP.Services
{
    public class AnimalServices
    {
        private readonly HttpClient _http;

        public AnimalServices(HttpClient http)
        {
            _http = http;
        }

        public async Task<List<Animal>> GetAllAnimalsForAdoption()
        {
            return await _http.GetFromJsonAsync<List<Animal>>("api/animals")
                   ?? new List<Animal>();
        }

        public async Task<Animal?> GetAnimalById(int id)
        {
            return await _http.GetFromJsonAsync<Animal>($"api/animals/{id}");
        }

        public async Task<int> AddNewAnimal(Animal animal)
        {
            var response = await _http.PostAsJsonAsync("api/animals", animal);
            response.EnsureSuccessStatusCode();

            var result = await response.Content.ReadFromJsonAsync<Dictionary<string, int>>();
            return result!["animalId"];
        }

        public async Task<bool> UpdateAnimal(int id, Animal animal)
        {
            var response = await _http.PutAsJsonAsync($"api/animals/{id}", animal);
            return response.IsSuccessStatusCode;
        }

        public async Task<bool> DeleteAnimal(int id)
        {
            var response = await _http.DeleteAsync($"api/animals/{id}");
            return response.IsSuccessStatusCode;
        }

        public async Task<List<string>> GetAllStatuses()
        {
            return await _http.GetFromJsonAsync<List<string>>("api/animals/statuses")
                   ?? new List<string>();
        }

        public async Task<List<Breed>> GetBreedsByTypeId(int typeId)
        {
            return await _http.GetFromJsonAsync<List<Breed>>($"api/animals/breeds/{typeId}")
                   ?? new List<Breed>();
        }

        public async Task<List<AnimalType>> GetAnimalTypes()
        {
            return await _http.GetFromJsonAsync<List<AnimalType>>("api/animals/animal-types")
                   ?? new List<AnimalType>();
        }
    }
}
