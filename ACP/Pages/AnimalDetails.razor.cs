//using Microsoft.AspNetCore.Components;
//using ACP.Models.Animals;
//using ACP.Services;
//using Microsoft.AspNetCore.Components.Authorization;
//using Microsoft.JSInterop;

//namespace ACP.Pages;

//public partial class AnimalDetails : ComponentBase
//{
//    [Parameter] public int AnimalId { get; set; }

//    [Inject] protected AnimalServices MyAnimalService { get; set; } = default!;
//    [Inject] protected NavigationManager Navigation { get; set; } = default!;
//    [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = default!;
//    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

//    protected Animal? AnimalItem;
//    protected bool isLoading = true;
//    protected int currentImageIndex = 0;

//    protected override async Task OnInitializedAsync()
//    {
//        try
//        {
//            isLoading = true;
//            AnimalItem = await MyAnimalService.GetAnimalById(AnimalId);
//        }
//        catch (Exception ex)
//        {
//            Console.WriteLine($"Error fetching data: {ex.Message}");
//        }
//        finally
//        {
//            isLoading = false;
//        }
//    }

//    protected async Task HandleAdoptionClick()
//    {
//        if (AnimalItem?.Status?.ToLower() != "available") return;

//        var authState = await AuthProvider.GetAuthenticationStateAsync();
//        var user = authState.User;

//        if (user.Identity is not { IsAuthenticated: true })
//        {
//            await JSRuntime.InvokeVoidAsync("alert", "Join our family first! Please sign up to proceed with adoption. 🐾");
//            Navigation.NavigateTo("SignUp");
//            return;
//        }

//        Navigation.NavigateTo($"confirm-adoption/{AnimalId}");
//    }

//    protected void NextImg()
//    {
//        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
//            currentImageIndex = (currentImageIndex + 1) % AnimalItem.animalImages.Count;
//    }

//    protected void PrevImg()
//    {
//        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
//            currentImageIndex = (currentImageIndex - 1 + AnimalItem.animalImages.Count) % AnimalItem.animalImages.Count;
//    }
//}


using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;
using ACP.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;
using System.Net.Http.Json; // تأكد من وجود هذا الـ namespace

namespace ACP.Pages;

public partial class AnimalDetails : ComponentBase
{
    [Parameter] public int AnimalId { get; set; }

    [Inject] protected AnimalServices MyAnimalService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected Animal? AnimalItem;
    protected bool isLoading = true;
    protected int currentImageIndex = 0;

    // المتغيرات الجديدة لعرض الأسماء نصياً
    protected string breedName = "---";
    protected string typeName = "---";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            AnimalItem = await MyAnimalService.GetAnimalById(AnimalId);

            if (AnimalItem != null)
            {
                // استدعاء دالة جلب الأسماء بعد التأكد من وجود الحيوان
                await LoadDetailedNames();
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching data: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    private async Task LoadDetailedNames()
    {
        try
        {
            // 1. جلب الأنواع ومطابقة النوع الحالي
            var types = await MyAnimalService.GetAnimalTypes();
            typeName = types?.FirstOrDefault(t => t.TypeId == AnimalItem.animalTypeId)?.TypeName ?? "Unknown Type";

            // 2. جلب السلالات بناءً على نوع الحيوان ومطابقة السلالة الحالية
            var breeds = await MyAnimalService.GetBreedsByTypeId(AnimalItem.animalTypeId);
            breedName = breeds?.FirstOrDefault(b => b.BreedId == AnimalItem.BreedId)?.BreedName ?? "Unknown Breed";
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error translating IDs to Names: {ex.Message}");
            typeName = "Unknown";
            breedName = "Unknown";
        }
    }

    protected async Task HandleAdoptionClick()
    {
        if (AnimalItem?.Status?.ToLower() != "available") return;

        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        if (user.Identity is not { IsAuthenticated: true })
        {
            await JSRuntime.InvokeVoidAsync("alert", "Join our family first! Please sign up to proceed with adoption. 🐾");
            Navigation.NavigateTo("SignUp");
            return;
        }

        Navigation.NavigateTo($"confirm-adoption/{AnimalId}");
    }

    protected void NextImg()
    {
        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
            currentImageIndex = (currentImageIndex + 1) % AnimalItem.animalImages.Count;
    }

    protected void PrevImg()
    {
        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
            currentImageIndex = (currentImageIndex - 1 + AnimalItem.animalImages.Count) % AnimalItem.animalImages.Count;
    }
}