using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Authorization;
using System.Net.Http.Json;
using static System.Net.WebRequestMethods;

namespace ACP.Features.Account
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _nav;
        private readonly AuthenticationStateProvider _authStateProvider;

        public AccountService(HttpClient httpClient, NavigationManager nav, AuthenticationStateProvider authenticationStateProvider)
        {
            _httpClient = httpClient;
            _nav = nav;
            _authStateProvider = authenticationStateProvider;
        }

        public async Task<AuthResponseDto?> Login(LoginRequest model)
        {

            var response = await _httpClient.PostAsJsonAsync("api/Account/login", model);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthResponseDto>();
            }

            return null;
        }

        public async Task<AuthModel> Register(Register model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Account/register", model);

            if (response.IsSuccessStatusCode)
            {
                return await response.Content.ReadFromJsonAsync<AuthModel>();
            }

            return new AuthModel { IsAuthenticated = false, Message = "حدث خطأ في الاتصال بالسيرفر" };
        }

        public async Task LogoutAsync()
        {
            try
            {
                // 1. إرسال طلب للباكيند لتدمير كوكي الـ Refresh Token وإلغائه من قاعدة البيانات
                // الـ CookieAndTokenHandler سيقوم تلقائياً بإرفاق التوكن الحالي والكوكيز مع هذا الطلب
                await _httpClient.PostAsync("api/Account/logout", null);
            }
            catch (System.Exception ex)
            {
                System.Console.WriteLine($"Logout API Error: {ex.Message}");
            }
            finally
            {
                // 2. تصفير الذاكرة وإعلام الواجهة بالخروج في الفرونت اند حتى لو فشل الاتصال بالباكيند
                if (_authStateProvider is CustomAuthStateProvider customProvider)
                {
                    customProvider.NotifyUserLogout();
                }
            }
        }

        public async Task<bool> CheckEmailAvailability(string email)
        {
            // نرسل الإيميل كـ Query String
            var response = await _httpClient.GetAsync($"api/Account/check-email?email={Uri.EscapeDataString(email)}");

            if (response.IsSuccessStatusCode)
            {
                // السيرفر يعيد true إذا كان متاحاً (Available)
                var isAvailable = await response.Content.ReadFromJsonAsync<bool>();
                return isAvailable;
            }

            // في حال فشل الاتصال، نفترض أنه غير متاح للحماية
            return false;
        }

        public async Task<bool> SendForgotPasswordLinkAsync(string email)
        {
            // إنشاء كائن الطلب ليتطابق مع الـ DTO المتوقع في الباكند
            var request = new { Email = email };

            // استدعاء الـ Endpoint الخاصة بك
            var response = await _httpClient.PostAsJsonAsync("api/account/forgot-password", request);

            // نرجع true إذا كان كود الحالة 200 OK (حتى لو لم يكن الإيميل مسجلاً لأسباب أمنية كما ذكرت)
            return response.IsSuccessStatusCode;
        }

        public async Task<ResetPasswordResponseDto> ResetPasswordAsync(object resetPasswordData)
        {
            var response = await _httpClient.PostAsJsonAsync("api/account/reset-password", resetPasswordData);

            if (response.IsSuccessStatusCode)
            {
                return new ResetPasswordResponseDto { IsSuccess = true };
            }

            // قراءة الأخطاء في حال فشل الطلب
            var errorResult = await response.Content.ReadFromJsonAsync<ErrorResponse>();
            return new ResetPasswordResponseDto
            {
                IsSuccess = false,
                Errors = errorResult?.Errors ?? new List<string> { "An error occurred. Please try again." }
            };
        }

        // كلاسات مساعدة لنقل البيانات وتلقي الأخطاء
        public class ResetPasswordResponseDto { public bool IsSuccess { get; set; } public List<string>? Errors { get; set; } }
        public class ErrorResponse { public List<string> Errors { get; set; } = new(); }
    }
}
