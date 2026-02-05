using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;
using ACP.Services;

namespace ACP.Pages;

public partial class BrowseAnimals
{
    [Inject] private AnimalServices AnimalService { get; set; } = default!;

    public List<Animal>? AnimalsList { get; set; }

    // المتغيرات التي كانت تسبب الخطأ
    private string searchText = "";
    private int selectedTypeId = 0;
    private string selectedAgeRange = "all";

    // منطق الفلترة الشامل (الاسم + النوع + العمر)
    private IEnumerable<Animal> FilteredAnimals =>
        (AnimalsList ?? new List<Animal>()).Where(a =>
            (string.IsNullOrWhiteSpace(searchText) || a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
            (selectedTypeId == 0 || a.animalTypeId == selectedTypeId) &&
            FilterByAge(a) // استدعاء دالة فلترة العمر
        );

    protected override async Task OnInitializedAsync()
    {
        try
        {
            var result = await AnimalService.GetAllAnimalsForAdoption();
            if (result != null) AnimalsList = result.ToList();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
            AnimalsList = new List<Animal>();
        }
    }

    // الدالة التي كانت تسبب الخطأ
    private void HandleAgeChange(string val)
    {
        selectedAgeRange = val;
        StateHasChanged();
    }

    private void HandleSearch(string val) { searchText = val; StateHasChanged(); }
    private void HandleTypeChange(int val) { selectedTypeId = val; StateHasChanged(); }

    private void HandleViewAnimalProfile(int? id) => Console.WriteLine($"ID: {id}");

    private int pageSize = 8; // عدد الكروت التي تظهر في كل مرة (مثلاً صفين)
    private int currentPageSize = 8; // القيمة الحالية المعروضة

    private void LoadMore()
{
    // نزيد العدد بمقدار 4 (صف إضافي في المتصفح)
    currentPageSize += 4;
    
    // لإعطاء إيحاء بالتحميل، يمكنك إضافة تأخير بسيط (اختياري)
    // StateHasChanged(); 
}

// دالة تصفير الفلتر التي كتبناها سابقاً يجب أن تصفر الـ Pagination أيضاً
private void ClearFilters()
{
    searchText = string.Empty;
    selectedTypeId = 0;
    selectedAgeRange = "all";
    currentPageSize = pageSize; // إعادة العرض للعدد الأصلي
    StateHasChanged();
}

    // منطق حساب العمر بناءً على تاريخ الميلاد في المودل
    private bool FilterByAge(Animal animal)
    {
        if (selectedAgeRange == "all") return true;

        var ageInYears = DateTime.Now.Year - animal.BirthDate.Year;

        return selectedAgeRange switch
        {
            "baby" => ageInYears <= 1,
            "young" => ageInYears > 1 && ageInYears <= 3,
            "adult" => ageInYears > 3,
            _ => true
        };
    }

    //private void ClearFilters()
    //{
    //    searchText = string.Empty;
    //    selectedTypeId = 0;
    //    selectedAgeRange = "all";
    //    StateHasChanged(); // هذا السطر يخبر الواجهة أن القيم تغيرت ليعيد رسم الكروت
    //}
}