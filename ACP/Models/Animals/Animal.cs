namespace ACP.Models.Animals
{
    public class Animal
    {
        public int? AnimalId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int animalTypeId { get; set; }
        public int? BreedId { get; set; }
        public string Color { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public string Nationality { get; set; }
        public string PassportNumber { get; set; }
        public DateTime BirthDate { get; set; }
        public DateTime? DateOfDeath { get; set; }
        public string Status { get; set; }
        public bool IsPet { get; set; } = false;
        public bool IsAnimalTame { get; set; } = false;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public int? CustomerID { get; set; }

        public ICollection<AnimalImage>? animalImages { get; set; }
        public ICollection<AnimalSurgicalOperation>? animalSurgicalOperations { get; set; }
        public ICollection<AnimalVaccination>? AnimalVaccination { get; set; }
    }


}

namespace ACP.Models.Animals
{
    public class AdoptionDetailDTO
    {
        public int? AnimalId { get; set; }
        public int CustomerId { get; set; }
        public string OperationStatus { get; set; } = string.Empty;
        public string? ReasonForRejection { get; set; }

    }
}
