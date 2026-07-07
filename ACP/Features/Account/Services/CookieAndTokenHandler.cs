using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.AspNetCore.Components.WebAssembly.Http;
using System.Net.Http.Headers;

namespace ACP.Features.Account
{
    public class CookieAndTokenHandler : DelegatingHandler
    {
        private readonly CustomAuthStateProvider _authStateProvider;

        public CookieAndTokenHandler(CustomAuthStateProvider authStateProvider)
        {
            _authStateProvider = authStateProvider;
        }

        protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request, CancellationToken cancellationToken)
        {
            // جلب التوكن يقيناً من الذاكرة المشتركة
            var token = _authStateProvider.GetTokenFromRam();

            Console.WriteLine($"[HandlerCheck] Target API: {request.RequestUri}. Token found in RAM: {!string.IsNullOrEmpty(token)}");

            if (!string.IsNullOrEmpty(token))
            {
                request.Headers.Authorization = new AuthenticationHeaderValue("Bearer", token);
            }

            // إرسال الكوكيز للباكيند
            request.SetBrowserRequestCredentials(BrowserRequestCredentials.Include);

            return await base.SendAsync(request, cancellationToken);
        }
    }
}
