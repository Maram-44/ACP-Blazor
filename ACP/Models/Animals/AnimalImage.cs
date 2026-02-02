namespace ACP.Models.Animals
{
    public class AnimalImage
    {
        public int ImageId { get; set; }
        public string Image { get; set; } = null!;
        public int AnimalId { get; set; }
    }
}
