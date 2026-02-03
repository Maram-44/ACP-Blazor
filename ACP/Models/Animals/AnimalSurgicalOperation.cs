

namespace ACP.Models.Animals
{
    public class AnimalSurgicalOperation
    {
        public string SurgicalOperation { get; set; } = null!;
        public int AnimalId { get; set; }
        public DateTime? Date { get; set; }
        public string? OperationStatus { get; set; }
    }
}
