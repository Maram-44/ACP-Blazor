using ACP.Models.Account;
using Blazored.LocalStorage;
using Microsoft.AspNetCore.Components;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class AccountService
    {
        private readonly HttpClient _httpClient;
        private readonly NavigationManager _nav;
        private readonly ILocalStorageService _localStorage;

        public AccountService(HttpClient httpClient, NavigationManager nav, ILocalStorageService localStorage)
        {
            _httpClient = httpClient;
            _nav = nav;
            _localStorage = localStorage;
        }

        public async Task<bool> Login(LoginRequest model)
        {
            var response = await _httpClient.PostAsJsonAsync("api/Account/login", model);

            if (response.IsSuccessStatusCode)
            {
                var result = await response.Content.ReadFromJsonAsync<AuthModel>();

                if (result != null && result.IsAuthenticated)
                {
                    // تخزين التوكن في المتصفح
                    await _localStorage.SetItemAsync("authToken", result.Token);

                    // يمكنك أيضاً تخزين الـ Refresh Token إذا أردتِ
                    await _localStorage.SetItemAsync("refreshToken", result.RefreshToken);

                    return true;
                }
            }
            return false;
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

        public async Task Logout()
        {
            // مسح التوكن عند تسجيل الخروج
            await _localStorage.RemoveItemAsync("authToken");
            await _localStorage.RemoveItemAsync("refreshToken");
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
