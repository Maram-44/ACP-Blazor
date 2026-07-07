using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ACP.Features.Account
{
    public class CustomerService
    {
        private readonly HttpClient _http;

        public CustomerService(HttpClient http) => _http = http;

        // 1. جلب بيانات الملف الشخصي
        public async Task<CustomerProfile?> GetProfileAsync()
        {
            var response = await _http.GetAsync("api/Customers/profile");
            return response.IsSuccessStatusCode
                ? await response.Content.ReadFromJsonAsync<CustomerProfile>()
                : null;
        }

        // 2. تحديث بيانات الملف الشخصي (ترجع رسالة الخطأ أو "Success")
        public async Task<string> UpdateProfileAsync(CustomerProfile dto)
        {
            // إنشاء الـ Form Content الذي سيحمل الحقول والملف معاً
            using var content = new MultipartFormDataContent();

            // أضافة الحقول النصية والأساسية (تأكدي من مطابقتها لحقول الموديل في الباكيند)
            content.Add(new StringContent(dto.UserId.ToString()), nameof(dto.UserId));
            content.Add(new StringContent(dto.UserName ?? string.Empty), nameof(dto.UserName));
            content.Add(new StringContent(dto.Email ?? string.Empty), nameof(dto.Email));
            content.Add(new StringContent(dto.PhoneNumber ?? string.Empty), nameof(dto.PhoneNumber));

            // حقول كلمة المرور إذا كان هناك تعديل عليها
            content.Add(new StringContent(dto.CurrentPassword ?? string.Empty), nameof(dto.CurrentPassword));
            content.Add(new StringContent(dto.NewPassword ?? string.Empty), nameof(dto.NewPassword));

            // إضافة بقية حقول الكستمر الخاصة بمشروعك (مثال لحقول شائعة)
            content.Add(new StringContent(dto.FirstName ?? string.Empty), nameof(dto.FirstName));
            content.Add(new StringContent(dto.MiddleName ?? string.Empty), nameof(dto.MiddleName));
            content.Add(new StringContent(dto.LastName ?? string.Empty), nameof(dto.LastName));
            content.Add(new StringContent(dto.Nationality ?? string.Empty), nameof(dto.Nationality));
            content.Add(new StringContent(dto.TypeOfIdentity ?? string.Empty), nameof(dto.TypeOfIdentity));
            content.Add(new StringContent(dto.IdentityNumber ?? string.Empty), nameof(dto.IdentityNumber));

            // 3. معالجة وحقن ملف الصورة إذا اختاره المستخدم من الواجهة
            if (dto.ImageFileBytes != null && dto.ImageFileBytes.Length > 0)
            {
                var fileContent = new ByteArrayContent(dto.ImageFileBytes);

                // تحديد نوع البيانات كـ Image
                fileContent.Headers.ContentType = new MediaTypeHeaderValue("image/jpeg");

                // تمرير الملف (الاسم يجب أن يطابق تماماً اسم الـ Property في الـ DTO للباكيند وهو ImageFile)
                content.Add(fileContent, "ImageFile", dto.ImageFileName ?? "profile_image.jpg");
            }

            // إرسال الطلب عبر الـ PUT
            var response = await _http.PutAsync("api/Customers/update-profile", content);

            if (response.IsSuccessStatusCode)
                return "Success";

            // قراءة رسالة الخطأ القادمة من الـ API (سواء كانت BadRequest نصية أو خطأ مخصص)
            return await response.Content.ReadAsStringAsync();
        }
    }
}
