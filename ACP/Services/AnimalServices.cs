using ACP.Models.Animals;
using System.Buffers.Text;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

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

 

        public async Task<List<Animal>> GetCustomerAnimalsAsync(int customerId)
        {
            try
            {
                // نرسل الطلب إلى المسار الذي حددناه في الـ API
                var response = await _http.GetFromJsonAsync<List<Animal>>($"api/Animals/my-animals/{customerId}");

                return response ?? new List<Animal>();
            }
            catch (Exception ex)
            {
                // معالجة الأخطاء في حال فشل الاتصال
                Console.WriteLine($"Error fetching animals: {ex.Message}");
                return new List<Animal>();
            }
        }








        //new
        public async Task<bool> AddNewAnimalWithImages(AnimalUpsertFormModel model)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // إضافة البيانات النصية
                content.Add(new StringContent(model.Name ?? ""), "Name");
                content.Add(new StringContent(model.Description ?? ""), "Description");
                content.Add(new StringContent(model.AnimalTypeId.ToString()), "AnimalTypeId");
                content.Add(new StringContent(model.Breed ?? ""), "Breed");
                content.Add(new StringContent(model.Color ?? ""), "Color");
                content.Add(new StringContent(model.Gender ?? ""), "Gender");
                content.Add(new StringContent(model.Country ?? ""), "Country");
                content.Add(new StringContent(model.City ?? ""), "City");
                content.Add(new StringContent(model.BirthDate.ToString("yyyy-MM-dd")), "BirthDate");

                if (model.ReturnDate.HasValue)
                {
                    content.Add(new StringContent(model.ReturnDate.Value.ToString("yyyy-MM-dd")), "ReturnDate");
                }

                if (!string.IsNullOrEmpty(model.PassportNumber))
                {
                    content.Add(new StringContent(model.PassportNumber), "PassportNumber");
                }

                // إرسال البايتات كملفات IFormFile للـ API
                if (model.SelectedImagesBytes != null && model.SelectedImagesBytes.Any())
                {
                    for (int i = 0; i < model.SelectedImagesBytes.Count; i++)
                    {
                        var fileContent = new ByteArrayContent(model.SelectedImagesBytes[i]);
                        fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                        // الاسم "Images" يجب أن يطابق تماماً الـ DTO في الـ API
                        content.Add(fileContent, "Images", $"animal_image_{i}_{Guid.NewGuid()}.jpg");
                    }
                }

                var response = await _http.PostAsync("api/animals/add", content);
                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Service Error: {ex.Message}");
                return false;
            }
        }
        public async Task<List<AnimalType>> GetAnimalTypes()
        {
            return await _http.GetFromJsonAsync<List<AnimalType>>("api/animals/animal-types")
                   ?? new List<AnimalType>();
        }
        // جلب حيوانات المستخدم الحالي (تتطلب توكن)
        public async Task<List<AnimalReadDto>> GetMyAnimalsAsync()
        {
            return await _http.GetFromJsonAsync<List<AnimalReadDto>>("api/animals/my-animals");
        }

        // جلب الحيوانات بنظام الـ Cursor (للتحميل اللانهائي أو التصفح)
        public async Task<List<AnimalReadDto>> GetAnimalsCursorAsync(int? lastId, int take = 6)
        {
            var url = $"api/animals/cursor?take={take}";
            if (lastId.HasValue) url += $"&lastId={lastId}";

            return await _http.GetFromJsonAsync<List<AnimalReadDto>>(url);
        }

        // جلب تفاصيل حيوان محدد
        public async Task<AnimalReadDto> GetAnimalByIdAsync(int id)
        {
            return await _http.GetFromJsonAsync<AnimalReadDto>($"api/animals/{id}");
        }
    }
}
