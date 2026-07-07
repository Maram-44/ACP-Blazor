namespace ACP.Features.Animals
{
    public class AnimalReadDto
    {
        public int AnimalId { get; set; }
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;
        public string Breed { get; set; } = null!;
        public string Color { get; set; } = null!;
        public string Gender { get; set; } = null!;
        public DateTime? ReturnDate { get; set; }
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Status { get; set; } = null!;
        public string AnimalType { get; set; } = null!;
        public DateTime BirthDate { get; set; }
        public int AnimalOwner { get; set; }
        public List<string> ImagePaths { get; set; } = new();
    }
}
