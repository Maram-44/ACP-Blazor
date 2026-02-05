namespace ACP.Models.Animals
{
    public class AdoptionDetail
    {
        public int? AnimalId { get; set; }
        public int CustomerId { get; set; }
        public string OperationStatus { get; set; } = null!;
        public string? ReasonForRejection { get; set; }

        public Animal Animal { get; set; }
    }
}
