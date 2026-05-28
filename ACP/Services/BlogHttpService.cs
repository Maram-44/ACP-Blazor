using ACP.Models.Blogs;
using Microsoft.AspNetCore.Components.Forms;
using System.Net.Http.Headers;
using System.Net.Http.Json;

namespace ACP.Services
{
    public class BlogHttpService
    {
        private readonly HttpClient _httpClient;
        private const string BaseUrl = "api/Blogs";

        public BlogHttpService(HttpClient httpClient)
        {
            _httpClient = httpClient;
        }

        // 1. جلب جميع المدونات (للزائر والمستخدم المسجل)
        public async Task<IEnumerable<ReadBlog>> GetAllBlogsAsync()
        {
            try
            {
                // الـ HttpClient الخاص بك يجب أن يكون مجهزاً لإرسال الـ Bearer Token تلقائياً في الـ Headers إذا كان موجوداً
                var response = await _httpClient.GetFromJsonAsync<IEnumerable<ReadBlog>>($"{BaseUrl}/all");
                return response ?? new List<ReadBlog>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching blogs: {ex.Message}");
                return new List<ReadBlog>();
            }
        }

        // 2. إرسال وتمرير المدونة الجديدة مع الصورة كـ Multipart Form Data
        public async Task<bool> CreateBlogAsync(AddBlogDTO dto, IBrowserFile? imageFile)
        {
            try
            {
                using var content = new MultipartFormDataContent();

                // إضافة النصوص العادية من الـ DTO
                content.Add(new StringContent(dto.Title ?? string.Empty), nameof(dto.Title));
                content.Add(new StringContent(dto.Content ?? string.Empty), nameof(dto.Content));

                // معالجة وإضافة ملف الصورة إذا تم اختياره من المتصفح
                if (imageFile != null)
                {
                    // تحديد الحد الأقصى المسموح به لحجم الصورة (مثلاً 5 ميجابايت لمنع الـ Exception الافتراضي لبليدزور)
                    long maxFileSize = 1024 * 1024 * 5;
                    var stream = imageFile.OpenReadStream(maxFileSize);

                    var fileContent = new StreamContent(stream);
                    fileContent.Headers.ContentType = new MediaTypeHeaderValue(imageFile.ContentType);

                    // نمرر حقل اسم الصورة ليتطابق مع ما يتوقعه الـ API [FromForm]
                    content.Add(fileContent, "ImageFile", imageFile.Name);
                }

                // إرسال الطلب إلى الـ Endpoint المحمية بـ Authorize
                var response = await _httpClient.PostAsync($"{BaseUrl}/create", content);

                return response.IsSuccessStatusCode;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error creating blog post: {ex.Message}");
                return false;
            }
        }

        public async Task<ReadBlog?> GetBlogByIdAsync(int id)
        {
            try
            {
                // إرسال طلب GET إلى السيرفر بالـ ID المحدد
                var response = await _httpClient.GetAsync($"api/Blogs/{id}");

                if (response.IsSuccessStatusCode)
                {
                    // قراءة البيانات وتحويلها لكائن الـ ReadBlog
                    return await response.Content.ReadFromJsonAsync<ReadBlog>();
                }

                return null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching blog details: {ex.Message}");
                return null;
            }
        }

        public async Task<int?> ToggleBlogVoteAsync(int blogId)
        {
            try
            {
                // استدعاء الـ API لتبديل حالة التصويت (إضافة أو إزالة)
                var response = await _httpClient.PostAsync($"api/blogs/{blogId}/toggle-vote", null);

                if (response.IsSuccessStatusCode)
                {
                    // قراءة عدد التصويتات الجديد القادم من السيرفر بعد التحديث
                    var updatedVoteCount = await response.Content.ReadFromJsonAsync<int>();
                    return updatedVoteCount;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error toggling vote: {ex.Message}");
            }

            return null; // في حال حدوث خطأ أو فشل العملية
        }
    }
}
