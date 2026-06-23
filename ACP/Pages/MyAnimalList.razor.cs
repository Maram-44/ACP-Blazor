using ACP.Models.Animals;
using ACP.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http.Json;
using System.Threading.Tasks;

namespace ACP.Pages
{
    public partial class MyAnimalList : ComponentBase
    {
        [Inject] public AnimalTransactionClientService TransactionService { get; set; } = default!;
        [Inject] public HttpClient Http { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;
        [Inject] public NavigationManager NavigationManager { get; set; } = default!;

        protected string selectedTab = "AllAnimals";
        protected int? currentCustomerId;
        protected List<Animal> AnimalsList = new();
        protected string? ErrorMessage;

        protected Dictionary<int, string> InputCodes = new();
        protected bool isRelinquishModalOpen = false;

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
                StateHasChanged();
            }
        }

        private async Task LoadData()
        {
            try
            {
                // 🟢 الموك داتا بدون أي كاونتر أو طلبات
                AnimalsList = new List<Animal>
                {
                    new Animal
                    {
                        AnimalId = 121, // الحيوان المطلوب بالتحديد
                        Name = "Max",
                        Status = "Foster",
                        BirthDate = DateTime.Now.AddYears(-2),
                        animalImages = new List<AnimalImage> { new AnimalImage { Image = "https://images.unsplash.com/photo-1543466835-00a7907e9de1?w=300" } }
                    },
                    new Animal
                    {
                        AnimalId = 122,
                        Name = "Bella",
                        Status = "Adoption",
                        BirthDate = DateTime.Now.AddMonths(-8),
                        animalImages = new List<AnimalImage> { new AnimalImage { Image = "https://images.unsplash.com/photo-1514888286974-6c03e2ca1dba?w=300" } }
                    },
                    new Animal
                    {
                        AnimalId = 123,
                        Name = "Charlie",
                        Status = "AtHome",
                        BirthDate = DateTime.Now.AddYears(-1).AddMonths(-3),
                        animalImages = new List<AnimalImage> { new AnimalImage { Image = "https://images.unsplash.com/photo-1444212477490-ca407925329e?w=300" } }
                    }
                };

                // تجهيز قاموس أكواد التسليم للحيوانات
                InputCodes.Clear();
                foreach (var animal in AnimalsList)
                {
                    int safeAnimalId = animal.AnimalId.HasValue ? animal.AnimalId.Value : 0;
                    InputCodes[safeAnimalId] = string.Empty;
                }

                ErrorMessage = null;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading mock data: {ex.Message}");
                ErrorMessage = "Failed to load custom simulation data.";
            }
        }

        protected async Task HandleReturnCode(int animalId)
        {
            if (!InputCodes.TryGetValue(animalId, out var code) || string.IsNullOrWhiteSpace(code))
            {
                await JSRuntime.InvokeVoidAsync("alert", "Please enter a valid 6-digit code.");
                return;
            }

            try
            {
                var requestPayload = new ConfirmCodeRequest
                {
                    TransactionId = animalId,
                    Code = code
                };

                var apiResponse = await TransactionService.ConfirmReturnAsync(requestPayload);

                if (apiResponse != null)
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Handover code confirmed successfully!");
                    InputCodes[animalId] = string.Empty;
                    await LoadData();
                }
                else
                {
                    await JSRuntime.InvokeVoidAsync("alert", "Failed to confirm code. Please check the code and try again.");
                }
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"An error occurred: {ex.Message}");
            }
        }

        protected void OpenRelinquishConfirmation()
        {
            isRelinquishModalOpen = true;
        }

        protected void CloseRelinquishConfirmation()
        {
            isRelinquishModalOpen = false;
        }

        protected void ConfirmRelinquish()
        {
            isRelinquishModalOpen = false;
            ChangeTab("RelinquishForm");
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
                return AnimalsList.Where(a => a.Status == "Foster");

            return AnimalsList;
        }

        protected string GetTagClass(string status) => status switch
        {
            "AtHome" => "bg-green-50 text-green-600 border border-green-100",
            "Foster" => "bg-orange-100 text-orange-600 border border-orange-100",
            _ => "bg-slate-50 text-slate-500 border border-slate-100"
        };

        protected void GoToRequestsDetails(int animalId)
        {
            if (animalId > 0)
            {
                NavigationManager.NavigateTo($"/requests-details/{animalId}");
            }
        }
    }
}