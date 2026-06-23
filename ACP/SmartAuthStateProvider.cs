//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;
//using System.Security.Claims;

//namespace ACP.Services // تأكدي من مطابقة اسم المشروع لديكِ
//{
//    public class SmartAuthStateProvider : AuthenticationStateProvider
//    {
//        private readonly IJSRuntime _jsRuntime;

//        public SmartAuthStateProvider(IJSRuntime jsRuntime)
//        {
//            _jsRuntime = jsRuntime;
//        }

//        public override async Task<AuthenticationState> GetAuthenticationStateAsync()
//        {
//            try
//            {
//                // جلب البيانات من ذاكرة المتصفح
//                var token = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "authToken");
//                var userId = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userId");
//                var userEmail = await _jsRuntime.InvokeAsync<string>("localStorage.getItem", "userEmail");

//                if (!string.IsNullOrEmpty(token) && !string.IsNullOrEmpty(userId))
//                {
//                    var identity = new ClaimsIdentity(new[]
//                    {
//                        new Claim(ClaimTypes.NameIdentifier, userId),
//                        new Claim(ClaimTypes.Name, userEmail ?? "User"),
//                        new Claim(ClaimTypes.Email, userEmail ?? ""),
//                        new Claim("AuthToken", token)
//                    }, "localStorage");

//                    var user = new ClaimsPrincipal(identity);
//                    return new AuthenticationState(user);
//                }
//            }
//            catch
//            {
//                // في حال وجود خطأ (مثل عدم تحميل JS بعد) نرجعه كضيف
//            }

//            var anonymous = new ClaimsPrincipal(new ClaimsIdentity());
//            return new AuthenticationState(anonymous);
//        }

//        public async Task LoginAsync(string userId, string userEmail, string token)
//        {
//            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "authToken", token);
//            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userId", userId);
//            await _jsRuntime.InvokeVoidAsync("localStorage.setItem", "userEmail", userEmail);

//            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//        }

//        public async Task LogoutAsync()
//        {
//            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "authToken");
//            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userId");
//            await _jsRuntime.InvokeVoidAsync("localStorage.removeItem", "userEmail");

//            NotifyAuthenticationStateChanged(GetAuthenticationStateAsync());
//        }
//    }
//}