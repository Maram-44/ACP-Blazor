using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ACP.Models.Animals
{
    public class AnimalUpsertFormModel
    {
        [Required(ErrorMessage = "Please enter the pet's name")]
        public string Name { get; set; } = string.Empty;

        [Required(ErrorMessage = "Description is required")]
        [MinLength(20, ErrorMessage = "Description should be at least 20 characters")]
        public string Description { get; set; } = string.Empty;

        [Range(1, 100, ErrorMessage = "Please select a category")]
        public int AnimalTypeId { get; set; }

        [Required(ErrorMessage = "Breed is required")]
        public string Breed { get; set; } = string.Empty;
        [Required(ErrorMessage = "Color is required")]
        public string Color { get; set; } = string.Empty;
        public string Gender { get; set; } = "Male";

        [Required(ErrorMessage = "Country is required")]
        public string Country { get; set; } = string.Empty;
        [Required(ErrorMessage = "City is required")]
        public string City { get; set; } = string.Empty;
        [Required(ErrorMessage = "Birth date is required")]
        public DateTime BirthDate { get; set; }
        public DateTime? ReturnDate { get; set; }
        public string? PassportNumber { get; set; }
        public List<byte[]> SelectedImagesBytes { get; set; } = new List<byte[]>();
        public List<IBrowserFile> SelectedImages { get; set; } = new();
    }
}
