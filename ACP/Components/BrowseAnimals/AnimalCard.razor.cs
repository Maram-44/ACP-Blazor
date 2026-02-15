using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;

namespace ACP.Components.BrowseAnimals;

public partial class AnimalCard
{
    [Parameter] public Animal AnimalItem { get; set; } = new();
    [Parameter] public EventCallback<int?> OnViewProfile { get; set; }

    // دالة لعرض نوع الحيوان (نصياً) بدل الرقم
    public string GetAnimalTypeName()
    {
        // هنا مستقبلاً يمكنك ربطها بجدول AnimalTypes
        // حالياً سنضع منطق بسيط للتجربة
        return AnimalItem.animalTypeId switch
        {
            1 => "Dog",
            2 => "Cat",
            3 => "Bird",
            _ => "Pet"
        };
    }

    public string GetDisplayImage()
    {
        if (AnimalItem.animalImages != null && AnimalItem.animalImages.Any())
        {
            var imgPath = AnimalItem.animalImages.First().Image;

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
            await OnViewProfile.InvokeAsync(AnimalItem.AnimalId);
        }
    }
}