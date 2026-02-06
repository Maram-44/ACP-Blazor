using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;
using ACP.Services;
using Microsoft.AspNetCore.Components.Authorization;
using Microsoft.JSInterop;

namespace ACP.Pages;

public partial class AnimalDetails
{
    [Parameter] public int AnimalId { get; set; }

    [Inject] private AnimalServices MyAnimalService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;
    [Inject] private AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] private IJSRuntime JSRuntime { get; set; } = default!;

    public Animal? AnimalItem;
    private bool isLoading = true;
    private int currentImageIndex = 0;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            // Fetching data from the service (Endpoint: api/animals/{id})
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

    private async Task HandleAdoptionClick()
    {
        // 1. Check animal status
        if (AnimalItem?.Status?.ToLower() != "available") return;

        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // 2. If not authenticated -> Redirect to SignUp page
        if (user.Identity is not { IsAuthenticated: true })
        {
            await JSRuntime.InvokeVoidAsync("alert", "Join our family first! Please sign up to proceed with adoption. 🐾");
            Navigation.NavigateTo("SignUp");
            return;
        }

        // 3. Since the profile page is not ready yet, we redirect directly to the confirmation
        // If the user is logged in, they are ready to adopt.
        Navigation.NavigateTo($"confirm-adoption/{AnimalId}");
    }

    private void NextImg()
    {
        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
            currentImageIndex = (currentImageIndex + 1) % AnimalItem.animalImages.Count;
    }

    private void PrevImg()
    {
        if (AnimalItem?.animalImages != null && AnimalItem.animalImages.Count > 0)
            currentImageIndex = (currentImageIndex - 1 + AnimalItem.animalImages.Count) % AnimalItem.animalImages.Count;
    }
}