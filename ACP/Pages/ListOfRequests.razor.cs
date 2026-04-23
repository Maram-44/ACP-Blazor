using ACP.Models.Animals;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System.Net.Http.Json;

namespace ACP.Pages
{
    public partial class ListOfRequests
    {
        [Inject] public HttpClient Http { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        // «” Œœ„Ì «·„Êœ·“ «·Œ«’… »ﬂ Â‰« „»«‘—…
        protected List<Animal> AdoptionRequests { get; set; } = new();
        protected List<Animal> FosterRequests { get; set; } = new();
        protected bool IsLoading { get; set; } = true;
        protected string ActiveTab { get; set; } = "Adoption";

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                var storedId = await JSRuntime.InvokeAsync<string>("localStorage.getItem", "customerId");

                if (!string.IsNullOrEmpty(storedId) && int.TryParse(storedId, out int id))
                {
                    await LoadApplications(id);
                }

                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadApplications(int customerId)
        {
            try
            {
                // „‰«œ«… «·‹ API «·ÕﬁÌﬁÌ
                var response = await Http.GetFromJsonAsync<List<Animal>>($"api/animals/customer/{customerId}");

                if (response != null)
                {
                    //  ﬁ”Ì„ «·»Ì«‰«  »‰«¡ ⁄·Ï «·‰Ê⁄ «·„ÊÃÊœ ›Ì «·„Êœ· Õﬁﬂ
                    AdoptionRequests = response.Where(a => a.Status == "Adoption").ToList();
                    FosterRequests = response.Where(a => a.Status == "Foster").ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }

        protected void ChangeTab(string tabName) => ActiveTab = tabName;
    }
}