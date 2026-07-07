namespace ACP.Features.Account
{
    public class Notification
    {
        public Guid? Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Title { get; set; } = null!;
        public string Message { get; set; } = null!;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public bool IsRead { get; set; } = false;
        public string? TargetUrl { get; set; }
    }
}
