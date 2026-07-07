using Microsoft.AspNetCore.Components.Forms;
using System.ComponentModel.DataAnnotations;

namespace ACP.Features.Animals
{
    public class AnimalUpsertFormModel : IValidatableObject
    {
        [Required(ErrorMessage = "Pet name is required.")]
        [StringLength(30, ErrorMessage = "Name cannot exceed 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\u0600-\u06FF]+$", ErrorMessage = "Name must contain letters only.")]
        public string Name { get; set; }

        [Required(ErrorMessage = "Please select a category.")]
        public int? AnimalTypeId { get; set; }

        [Required(ErrorMessage = "Breed is required.")]
        [StringLength(35, ErrorMessage = "Breed cannot exceed 35 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\u0600-\u06FF]+$", ErrorMessage = "Breed must contain letters only.")]
        public string Breed { get; set; }

        [Required(ErrorMessage = "Birth date is required.")]
        public DateTime? BirthDate { get; set; } = DateTime.Today;

        [Required(ErrorMessage = "Country is required.")]
        public string Country { get; set; }

        [Required(ErrorMessage = "City is required.")]
        [StringLength(35, ErrorMessage = "City name cannot exceed 35 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\u0600-\u06FF]+$", ErrorMessage = "City must contain letters only.")]
        public string City { get; set; }

        [StringLength(20, ErrorMessage = "Passport number cannot exceed 20 characters.")]
        public string PassportNumber { get; set; }

        [Required(ErrorMessage = "Color is required.")]
        [StringLength(30, ErrorMessage = "Color cannot exceed 30 characters.")]
        [RegularExpression(@"^[a-zA-Z\s\u0600-\u06FF]+$", ErrorMessage = "Color must contain letters only.")]
        public string Color { get; set; }

        [Required(ErrorMessage = "Gender is required.")]
        public string Gender { get; set; } = "Male";

        [Required(ErrorMessage = "Description is required.")]
        [StringLength(500, MinimumLength = 20, ErrorMessage = "Description must be between 20 and 500 characters.")]
        public string Description { get; set; }

        public DateTime? ReturnDate { get; set; }

        public List<byte[]> SelectedImagesBytes { get; set; } = new();

        // التحقق المتقدم والديناميكي من التواريخ
        public IEnumerable<ValidationResult> Validate(ValidationContext validationContext)
        {
            if (SelectedImagesBytes == null || SelectedImagesBytes.Count == 0)
            {
                yield return new ValidationResult("You must upload at least one photo for the pet.", new[] { nameof(SelectedImagesBytes) });
            }

            // 1. فحص تاريخ الميلاد ليكون مناسباً للحيوانات الأليفة (بين اليوم وقبل 25 سنة)
            if (BirthDate.HasValue)
            {
                if (BirthDate.Value > DateTime.Today)
                {
                    yield return new ValidationResult("Birth date cannot be in the future.", new[] { nameof(BirthDate) });
                }
                else if (BirthDate.Value < DateTime.Today.AddYears(-25))
                {
                    yield return new ValidationResult("Age cannot exceed 25 years for pets.", new[] { nameof(BirthDate) });
                }
            }

            // 2. فحص تاريخ العودة للاستضافة المؤقتة (يتم تفعيل هذا الشرط برمجياً عبر الكومبوننت أو هنا)
            // ملاحظة: بما أن حقل الـ isFoster موجود في الكومبوننت، نقوم بفحص الـ ReturnDate إذا لم يكن فارغاً
            if (ReturnDate.HasValue)
            {
                var minimumAllowedDate = DateTime.Today.AddDays(2);
                if (ReturnDate.Value.Date < minimumAllowedDate)
                {
                    yield return new ValidationResult("Expected return date must be at least 2 days from today.", new[] { nameof(ReturnDate) });
                }
            }
        }
    }
    }
