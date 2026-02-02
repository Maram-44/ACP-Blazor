


namespace ACP.Models.Animals
{
    public class AnimalVaccination
    {
        public int SurgicalOperationId { get; set; }
        public string SurgicalOperation { get; set; } = null!;
        public int AnimalId { get; set; }
        public DateTime? Date { get; set; }
        public string OperationStatus { get; set; } = null!;

    }
}
