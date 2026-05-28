using Microsoft.AspNetCore.Http;

namespace ACP.Models.Blogs
{
    public class AddBlogDTO
    {
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public IFormFile? ImageFile { get; set; }
    }
}
