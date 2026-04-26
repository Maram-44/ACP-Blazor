using ACP.Models.Account;
using Blazored.LocalStorage;
using Microsoft.Extensions.Configuration;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ACP
{
    public class JwtHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;
        private readonly IConfiguration _configuration;

        public JwtHandler(ILocalStorageService localStorage, IConfiguration configuration)
        {
            _localStorage = localStorage;
            _configuration = configuration;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // 1. جلب التوكن الحالي وإضافته للطلب
            var token = await _localStorage.GetItemAsync<string>("authToken");
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // 2. إرسال الطلب الأصلي
            var response = await base.SendAsync(request, cancellationToken);

            // 3. إذا انتهى التوكن (Unauthorized)
            if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
            {
                var refreshToken = await _localStorage.GetItemAsync<string>("refreshToken");

                if (!string.IsNullOrEmpty(refreshToken))
                {
                    // محاولة التجديد
                    var result = await RefreshToken(token, refreshToken);

                    if (result != null && result.IsAuthenticated)
                    {
                        // 4. تحديث التوكن في الطلب وإعادة المحاولة لمرة واحدة
                        request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", result.Token);

                        // التخلص من الاستجابة القديمة لفتح قناة جديدة
                        response.Dispose();
                        return await base.SendAsync(request, cancellationToken);
                    }
                }
            }

            return response;
        }

        private async Task<AuthModel> RefreshToken(string token, string refreshToken)
        {
            try
            {
                // نستخدم HttpClient جديد تماماً هنا لتجنب "الحلقة اللانهائية" (Circular Dependency)
                using var client = new HttpClient();
                var apiUrl = $"{_configuration["url"]}/api/account/refresh-token";

                var refreshRequest = new { Token = token, RefreshToken = refreshToken };
                var response = await client.PostAsJsonAsync(apiUrl, refreshRequest);

                if (response.IsSuccessStatusCode)
                {
                    var authModel = await response.Content.ReadFromJsonAsync<AuthModel>();

                    if (authModel != null && authModel.IsAuthenticated)
                    {
                        // حفظ التوكنات الجديدة في المتصفح
                        await _localStorage.SetItemAsync("authToken", authModel.Token);
                        await _localStorage.SetItemAsync("refreshToken", authModel.RefreshToken);
                        return authModel;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refresh Token Failed: {ex.Message}");
            }

            return null;
        }
    }
}