using Microsoft.AspNetCore.Components.Authorization;

namespace ACP.Features.Account
{
    public class UserSession
    {
        private readonly AuthenticationStateProvider _authStateProvider;

        public UserSession(AuthenticationStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        // خاصية تجلب الـ CustomerId فوراً كـ Int
        public async Task<int?> GetCustomerIdAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var claimValue = authState.User.FindFirst("CustomerId")?.Value;

            return int.TryParse(claimValue, out var id) ? id : null;
        }

        // خاصية تفحص حالة الملف الشخصي
        public async Task<bool> IsProfileCompletedAsync()
        {
            var authState = await _authStateProvider.GetAuthenticationStateAsync();
            var claimValue = authState.User.FindFirst("IsProfileCompleted")?.Value;

            return claimValue == "true";
        }
    }
}
