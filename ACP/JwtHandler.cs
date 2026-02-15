using Blazored.LocalStorage;
using System.Net.Http.Headers;

namespace ACP
{
    public class JwtHandler : DelegatingHandler
    {
        private readonly ILocalStorageService _localStorage;

        public JwtHandler(ILocalStorageService localStorage)
        {
            _localStorage = localStorage;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // جلب التوكن من التخزين المحلي
            var token = await _localStorage.GetItemAsync<string>("authToken");

            // إذا كان التوكن موجوداً، أضفه للطلب
            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
