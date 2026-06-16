//using Microsoft.AspNetCore.Components;
//using ACP.Models.Animals;
//using ACP.Services;

//namespace ACP.Pages;

//public partial class BrowseAnimals
//{
//    // 1. حقن الخدمات المطلوبة
//    [Inject] private AnimalServices AnimalService { get; set; } = default!;
//    [Inject] private NavigationManager Navigation { get; set; } = default!;

//    // القوائم التي سيتم جلبها من الـ API
//    public List<Animal>? AnimalsList { get; set; }
//    public List<AnimalType>? AnimalTypes { get; set; } // قائمة الأنواع الديناميكية

//    // المتغيرات الخاصة بالفلترة
//    private string searchText = "";
//    private int selectedTypeId = 0;
//    private string selectedAgeRange = "all";

//    // المتغيرات الخاصة بالـ Pagination
//    private int pageSize = 8;
//    private int currentPageSize = 8;

//    // منطق الفلترة الشامل (الاسم + النوع + العمر)
//    private IEnumerable<Animal> FilteredAnimals =>
//        (AnimalsList ?? new List<Animal>()).Where(a =>
//            (string.IsNullOrWhiteSpace(searchText) || a.Name.Contains(searchText, StringComparison.OrdinalIgnoreCase)) &&
//            (selectedTypeId == 0 || a.animalTypeId == selectedTypeId) &&
//            FilterByAge(a)
//        );

//    protected override async Task OnInitializedAsync()
//    {
//        try
//        {
//            // جلب البيانات من الـ API بشكل متوازي لتحسين الأداء
//            var animalsTask = AnimalService.GetAllAnimalsForAdoption();
//            var typesTask = AnimalService.GetAnimalTypes(); 

//            await Task.WhenAll(animalsTask, typesTask);

//            if (animalsTask.Result != null)
//            {
//                AnimalsList = animalsTask.Result.ToList();
//            }

//            if (typesTask.Result != null)
//            {
//                AnimalTypes = typesTask.Result.ToList();
//            }
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error fetching data from API: {ex.Message}");
//            AnimalsList ??= new List<Animal>();
//            AnimalTypes ??= new List<AnimalType>();
//        }
//    }

//    // التعامل مع تغييرات الفلاتر
//    private void HandleSearch(string val) { searchText = val; ResetPagination(); }
//    private void HandleTypeChange(int val) { selectedTypeId = val; ResetPagination(); }
//    private void HandleAgeChange(string val) { selectedAgeRange = val; ResetPagination(); }

//    // إعادة تصغير القائمة عند تغيير الفلتر
//    private void ResetPagination()
//    {
//        currentPageSize = pageSize;
//        StateHasChanged();
//    }

//    // منطق الـ Load More
//    private void LoadMore()
//    {
//        currentPageSize += 4;
//        StateHasChanged();
//    }

//    // تصفير الفلاتر والعودة للعدد الأصلي
//    private void ClearFilters()
//    {
//        searchText = string.Empty;
//        selectedTypeId = 0;
//        selectedAgeRange = "all";
//        currentPageSize = pageSize;
//        StateHasChanged();
//    }

//    // منطق حساب العمر بناءً على تاريخ الميلاد
//    private bool FilterByAge(Animal animal)
//    {
//        if (selectedAgeRange == "all") return true;

//        // حساب العمر بدقة (السنوات)
//        var ageInYears = DateTime.Now.Year - animal.BirthDate.Year;
//        if (animal.BirthDate.Date > DateTime.Now.AddYears(-ageInYears)) ageInYears--;

//        return selectedAgeRange switch
//        {
//            "baby" => ageInYears <= 1,
//            "young" => ageInYears > 1 && ageInYears <= 3,
//            "adult" => ageInYears > 3,
//            _ => true
//        };
//    }

//    // الانتقال لصفحة البروفايل (التفاصيل)
//    private void HandleViewAnimalProfile(int? id)
//    {
//        if (id.HasValue)
//        {
//            Navigation.NavigateTo($"/animal-details/{id.Value}");
//        }
//    }
//}