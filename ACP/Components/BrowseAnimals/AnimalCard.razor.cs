using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;

namespace ACP.Components.BrowseAnimals;

public partial class AnimalCard
{
    // 1. تغيير نوع المعامل هنا ليستقبل الـ DTO الجديد بدلاً من الموديل القديم
    [Parameter] public AnimalReadDto AnimalItem { get; set; } = new();

    // 2. تحديث الـ EventCallback ليمرر الكائن الكامل أو الـ ID لتتوافق مع الصفحة الرئيسية
    [Parameter] public EventCallback<AnimalReadDto> OnViewProfile { get; set; }

    // دالة لعرض نوع الحيوان (نصياً) مباشرة من الـ DTO
    public string GetAnimalTypeName()
    {
        // الـ DTO الجديد يحتوي بالفعل على اسم النوع كـ string جاهز (AnimalType)
        return !string.IsNullOrEmpty(AnimalItem.AnimalType) ? AnimalItem.AnimalType : "Pet";
    }

    public string GetDisplayImage()
    {
        // الـ DTO الجديد يحتوي على قائمة مسارات الصور باسم ImagePaths
        if (AnimalItem.ImagePaths != null && AnimalItem.ImagePaths.Any())
        {
            var imgPath = AnimalItem.ImagePaths.First();

            // إذا كان الرابط يبدأ بـ http فهو رابط كامل، وإلا أضف رابط السيرفر
            return imgPath.StartsWith("http") ? imgPath : $"https://your-api-url.com/{imgPath}";
        }
        return "images/default-pet.png";
    }

    public string GetAge()
    {
        var ageInYears = DateTime.Now.Year - AnimalItem.BirthDate.Year;
        if (ageInYears <= 0) return "Less than a year";
        return ageInYears == 1 ? "1 Year" : $"{ageInYears} Years";
    }

    private async Task HandleClick()
    {
        if (OnViewProfile.HasDelegate)
        {
            // نمرر الكائن بالكامل ليتوافق مع توقيع الدالة HandleViewAnimalProfile(AnimalReadDto animal) في الصفحة الرئيسية
            await OnViewProfile.InvokeAsync(AnimalItem);
        }
    }
}