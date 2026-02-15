using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;
using ACP.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

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

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            AnimalItem = await MyAnimalService.GetAnimalById(AnimalId);
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