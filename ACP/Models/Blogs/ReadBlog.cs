namespace ACP.Models.Blogs
{
    public class ReadBlog
    {
        public int Id { get; set; }
        public string Title { get; set; } = null!;
        public string Content { get; set; } = null!;
        public string? ImagePath { get; set; }
        public int NumberOfVote { get; set; }
        public DateTime PublicationDate { get; set; }
        public bool IsVoted { get; set; }
    }
}
