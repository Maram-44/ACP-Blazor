using ACP.Models.Animals;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ACP.Pages
{
    public partial class MyAnimalList
    {
        [Inject] public HttpClient Http { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        private string selectedTab = "MyAnimals";
        private int? currentCustomerId;
        private List<Animal> AnimalsList = new(); // تبدأ فارغة تماماً

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                // سحب الـ ID من Local Storage عند أول تحميل للصفحة
                var storedId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "customerId");

                if (!string.IsNullOrEmpty(storedId) && int.TryParse(storedId, out int id))
                {
                    currentCustomerId = id;
                    await LoadRealData();
                    StateHasChanged(); // تحديث الواجهة بعد جلب البيانات
                }
            }
        }

        private async Task LoadRealData()
        {
            try
            {
                // استدعاء السيرفر لجلب حيوانات هذا العميل فقط
                // تأكدي أن الـ Endpoint في الـ API تطابق هذا المسار
                var result = await Http.GetFromJsonAsync<List<Animal>>($"api/animals/customer/{currentCustomerId}");
                if (result != null)
                {
                    AnimalsList = result;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading animals: {ex.Message}");
                AnimalsList = new List<Animal>();
            }
        }

        private void ChangeTab(string tabName) => selectedTab = tabName;

        private IEnumerable<Animal> GetFilteredAnimals()
        {
            if (AnimalsList == null) return Enumerable.Empty<Animal>();

            // الفلترة الصارمة بناءً على التبويب والـ ID
            return selectedTab switch
            {
                "MyAnimals" => AnimalsList.Where(a => a.Status != "Relinquished"),
                "Relinquished" => AnimalsList.Where(a => a.Status == "Relinquished"),
                "CareHistory" => AnimalsList.Where(a => a.animalSurgicalOperations?.Any() == true),
                _ => Enumerable.Empty<Animal>()
            };
        }

        private string GetTagClass(string status) => status switch
        {
            "Healthy" => "bg-green-50 text-green-600 border border-green-100",
            "Pending" => "bg-orange-100 text-orange-600 border border-orange-100",
            "Relinquished" => "bg-slate-100 text-slate-600 border border-slate-200",
            _ => "bg-gray-50 text-gray-600"
        };
    }
}