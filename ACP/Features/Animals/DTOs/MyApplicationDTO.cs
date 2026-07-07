namespace ACP.Features.Animals
{
    public class MyApplicationDTO
    {
        public int ApplicationId { get; set; }
        public int AnimalId { get; set; }
        public string AnimalName { get; set; } = string.Empty;
        public string AnimalStatus { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? AnimalImage { get; set; } // أول صورة للحيوان
        public DateTime ApplicationDate { get; set; }
        public string Status { get; set; } = string.Empty; // Pending, Approved, Rejected
        public string? ReasonOfApplication { get; set; }
        public string OwnerName { get; set; } = string.Empty; // اسم صاحب الحيوان
    }
}
