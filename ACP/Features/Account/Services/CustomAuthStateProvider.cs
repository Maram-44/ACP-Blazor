using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.IdentityModel.Tokens.Jwt;
using System.Net.Http.Json;
using System.Security.Claims;
using System.Threading.Tasks;
using System;
using System.Collections.Generic;

namespace ACP.Features.Account
{
    public class CustomAuthStateProvider : AuthenticationStateProvider
    {
        private readonly HttpClient _httpClient;
        private readonly AuthenticationState _anonymous;
        private static string _accessToken = string.Empty; // 👈 تحويله إلى static يضمن أن أي نسخة تقرأ نفس التوكن يقيناً
        private Task<AuthenticationState>? _refreshTask;

        public CustomAuthStateProvider(HttpClient httpClient)
        {
            _httpClient = httpClient;
            _anonymous = new AuthenticationState(new ClaimsPrincipal(new ClaimsIdentity()));
        }

        // دالة ثابتة لاسترجاع التوكن من أي مكان في التطبيق
        public string GetTokenFromRam() => _accessToken;

        public override Task<AuthenticationState> GetAuthenticationStateAsync()
        {
            if (!string.IsNullOrEmpty(_accessToken))
            {
                return Task.FromResult(BuildAuthenticationState(_accessToken));
            }

            if (_refreshTask == null)
            {
                _refreshTask = ExecutingRefreshAsync();
            }

            return _refreshTask;
        }

        private async Task<AuthenticationState> ExecutingRefreshAsync()
        {
            try
            {
                var request = new HttpRequestMessage(HttpMethod.Post, "api/Account/refresh");
                request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

                var response = await _httpClient.SendAsync(request);

                if (response.IsSuccessStatusCode)
                {
                    var result = await response.Content.ReadFromJsonAsync<AuthResponseDto>();
                    if (result != null && !string.IsNullOrEmpty(result.Token))
                    {
                        _accessToken = result.Token; // شحن الذاكرة الموحدة
                        return BuildAuthenticationState(_accessToken);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Refresh Error: {ex.Message}");
                return _anonymous;
            }
            finally
            {
                _refreshTask = null;
            }

            return _anonymous;
        }

        public void NotifyUserLogin(string token)
        {
            _accessToken = token;
            _refreshTask = null;
            NotifyAuthenticationStateChanged(Task.FromResult(BuildAuthenticationState(token)));
        }

        public void NotifyUserLogout()
        {
            _accessToken = string.Empty;
            _refreshTask = null;
            NotifyAuthenticationStateChanged(Task.FromResult(_anonymous));
        }

        private AuthenticationState BuildAuthenticationState(string token)
        {
            var handler = new JwtSecurityTokenHandler();
            var jwtToken = handler.ReadJwtToken(token);
            var identity = new ClaimsIdentity(jwtToken.Claims, "Bearer"); // استخدام Bearer صراحة
            return new AuthenticationState(new ClaimsPrincipal(identity));
        }
    }

    // الموديل المتوافق مع الـ JSON القادم من الباكيند الخاص بك
    public class AuthResponseDto
    {
        public string Token { get; set; } = string.Empty;
        public List<string> Roles { get; set; } = new();
    }
}