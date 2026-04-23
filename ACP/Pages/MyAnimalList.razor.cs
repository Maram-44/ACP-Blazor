using ACP.Models.Animals;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ACP.Pages
{
    public partial class MyAnimalList : ComponentBase
    {
        [Inject] public HttpClient Http { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        // جعلنا القيم محمية (protected) لضمان وصول ملف الـ Razor لها
        protected string selectedTab = "AllAnimals";
        protected int? currentCustomerId;
        protected List<Animal> AnimalsList = new();
        protected string ErrorMessage;

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
           
                var storedId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "customerId");

                if (!string.IsNullOrEmpty(storedId) && int.TryParse(storedId, out int id))
                {
                    currentCustomerId = id;
                }

                await LoadData();

                // إجبار الصفحة على إعادة الرسم
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            try
            {
                // محاولة جلب بيانات حقيقية إذا كان الـ ID موجود
                if (currentCustomerId.HasValue)
                {
                    var result = await Http.GetFromJsonAsync<List<Animal>>($"api/animals/customer/{currentCustomerId}");
                    if (result != null && result.Any())
                    {
                        AnimalsList = result;
                        return; // الخروج إذا نجح التحميل
                    }
                }
                else
                {
                    // الحالة المطلوبة: إذا كان الـ API رد بنجاح لكن لا توجد بيانات (فاضي)
                    AnimalsList = new List<Animal>();
                    ErrorMessage = "No animals found in your account.";
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"API Error: {ex.Message}");
            }

        }


        protected void ChangeTab(string tabName)
        {
            selectedTab = tabName;
            StateHasChanged();
        }

        protected IEnumerable<Animal> GetFilteredAnimals()
        {
            if (AnimalsList == null || !AnimalsList.Any()) return Enumerable.Empty<Animal>();

            if (selectedTab == "AllAnimals") return AnimalsList;

            if (selectedTab == "Adoption")
                return AnimalsList.Where(a => a.Status == "AtHome" || a.Status == "Adoption");

            if (selectedTab == "Foster")
                return AnimalsList.Where(a => a.Status == "Foster" || a.Status == "Foster");

            return AnimalsList;
        }

        protected string GetTagClass(string status) => status switch
        {
            "AtHome" => "bg-green-50 text-green-600 border border-green-100",
            "Foster" => "bg-orange-100 text-orange-600 border border-orange-100",
            _ => "bg-slate-50 text-slate-500 border border-slate-100"
        };
    }
}