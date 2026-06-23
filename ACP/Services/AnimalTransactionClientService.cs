using ACP.Models;
using ACP.Models.Animals;
using ACP.Models.Animals;
using System.Buffers.Text;
using System.Net.Http;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class AnimalTransactionClientService
    {
        private readonly HttpClient _http;
        private const string BasePath = "api/AnimalTransaction";

        public AnimalTransactionClientService(HttpClient http)
        {
            _http = http;
        }

        public async Task<ContactDetailsDTO?> GetAcceptedApplicantDetailsAsync(int animalId)
        {
            try
            {
                // استدعاء الـ Endpoint التي قمنا بإنشائها في الباكيند
                var response = await _http.GetAsync($"api/AnimalTransaction/accepted-applicant/{animalId}");

                if (response.StatusCode == System.Net.HttpStatusCode.NotFound)
                {
                    return null; // لا توجد معاملة معلقة، وهذا طبيعي جداً قبل قبول أي طلب
                }

                // قراءة وتفسير البيانات في حال النجاح
                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<ContactDetailsDTO>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching accepted applicant details: {ex.Message}");
                return null;
            }
        }

        // 1. تقديم طلب لتبني أو رعاية
        public async Task<ApiResponse<string>?> SubmitApplicationAsync(SubmitApplicationRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BasePath}/submit-application", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        // 2. جلب جميع المتقدمين لحيوان معين
        public async Task<List<ApplicantDTO>?> GetApplicantsForAnimalAsync(int animalId)
        {
            // هنا الـ Controller يرجع Ok(applicants) مباشرة (قائمة)
            return await _http.GetFromJsonAsync<List<ApplicantDTO>>($"{BasePath}/applicants/{animalId}");
        }

        // 3. جلب تفاصيل متقدم واحد
        public async Task<ApplicantDTO?> GetApplicantDetailsAsync(int applicationId)
        {
            return await _http.GetFromJsonAsync<ApplicantDTO>($"{BasePath}/applicant-details/{applicationId}");
        }

        // 4. قبول طلب متقدم (يرجع بيانات التواصل)
        public async Task<ApiResponse<ContactDetailsDTO>?> AcceptApplicantAsync(int applicationId)
        {
            var response = await _http.PostAsync($"{BasePath}/accept-applicant/{applicationId}", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse<ContactDetailsDTO>>();
        }

        public async Task<List<MyApplicationDTO>> GetMyApplicationsAsync()
        {
            try
            {
                // سيقوم الـ HttpClient تلقائياً بإرسال الـ JWT Token 
                // إذا كان الـ Handler مبرمجاً على ذلك في Program.cs
                var response = await _http.GetAsync($"{BasePath}/my-applications");

                if (response.IsSuccessStatusCode)
                {
                    return await response.Content.ReadFromJsonAsync<List<MyApplicationDTO>>()
                           ?? new List<MyApplicationDTO>();
                }

                return new List<MyApplicationDTO>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"خطأ أثناء جلب طلباتي: {ex.Message}");
                return new List<MyApplicationDTO>();
            }
        }
 

        // 5. رفض طلب متقدم
        public async Task<ApiResponse<string>?> RejectApplicantAsync(int applicationId)
        {
            var response = await _http.PostAsync($"{BasePath}/reject-applicant/{applicationId}", null);
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        // 6. تأكيد تسليم الحيوان (استخدام كود المتبني)
        public async Task<ApiResponse<string>?> ConfirmDeliveryAsync(ConfirmCodeRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BasePath}/confirm-delivery", request);
            response.EnsureSuccessStatusCode();
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        // 7. تأكيد إرجاع الحيوان (استخدام كود المالك)
        public async Task<ApiResponse<string>?> ConfirmReturnAsync(ConfirmCodeRequest request)
        {
            var response = await _http.PostAsJsonAsync($"{BasePath}/confirm-return", request);
            return await response.Content.ReadFromJsonAsync<ApiResponse<string>>();
        }

        public async Task<bool> CheckIfAlreadyAppliedAsync(int animalId)
        {
            return await _http.GetFromJsonAsync<bool>($"{BasePath}/check-applied/{animalId}");
        }
    }
}
