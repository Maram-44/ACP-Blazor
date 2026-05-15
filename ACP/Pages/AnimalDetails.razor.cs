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
    [Inject] protected CustomerService MyCustomerService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;

    protected AnimalReadDto? AnimalItem;
    protected bool isLoading = true;
    protected int currentImageIndex = 0;

    // متغيرات التحكم في الرسالة المنبثقة (Modal)
    protected bool showProfileModal = false;
    protected bool isLoggedIn = false;
    protected string modalType = "LOGIN"; // أو "PROFILE" لتمييز الرسالة

    protected override async Task OnInitializedAsync()
    {
        await LoadAnimal();
    }

    private async Task LoadAnimal()
    {
        try
        {
            isLoading = true;
            if (AnimalId > 0)
            {
                AnimalItem = await MyAnimalService.GetAnimalByIdAsync(AnimalId);
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error fetching animal: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
    }

    protected async Task HandleAdoptionClick()
    {
        if (AnimalItem == null) return;

        var authState = await AuthProvider.GetAuthenticationStateAsync();
        var user = authState.User;

        // 1. التحقق من تسجيل الدخول
        if (user?.Identity == null || !user.Identity.IsAuthenticated)
        {
            isLoggedIn = false;
            modalType = "LOGIN";
            showProfileModal = true; // إظهار المودال بدلاً من الـ Alert
            return;
        }

        isLoggedIn = true;

        try
        {
            // 2. التحقق من اكتمال البروفايل
            var profile = await MyCustomerService.GetProfileAsync();

            if (profile == null)
            {
                modalType = "PROFILE";
                showProfileModal = true;
                return;
            }

            // التحقق من الحقول الإجبارية
            bool isDataComplete = !string.IsNullOrWhiteSpace(profile.PhoneNumber) &&
                                  !string.IsNullOrWhiteSpace(profile.Nationality) &&
                                  !string.IsNullOrWhiteSpace(profile.TypeOfIdentity) &&
                                  !string.IsNullOrWhiteSpace(profile.ImageWithIdentity) &&
                                  !string.IsNullOrWhiteSpace(profile.IdentityNumber);

            if (!isDataComplete)
            {
                modalType = "PROFILE";
                showProfileModal = true;
                return;
            }

            // 3. التوجيه بناءً على الحالة (إذا كان كل شيء مكتمل)
            ProceedToBooking();
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error: {ex.Message}");
        }
    }

    // دالة التوجيه للأكشن النهائي
    private void ProceedToBooking()
    {
        string status = AnimalItem?.Status?.ToUpper() ?? "";
        if (status.Contains("FOSTER"))
        {
            Navigation.NavigateTo($"foster-care-request/{AnimalId}");
        }
        else
        {
            Navigation.NavigateTo($"confirm-adoption/{AnimalId}");
        }
    }

    // دالة الزر داخل المودال
    protected void NavigateToAction()
    {
        showProfileModal = false;
        if (modalType == "LOGIN")
        {
            Navigation.NavigateTo("SignUp");
        }
        else
        {
            Navigation.NavigateTo("profile");
        }
    }

    protected string CalculateAge(DateTime birthDate)
    {
        var today = DateTime.Today;
        var age = today.Year - birthDate.Year;
        if (birthDate.Date > today.AddYears(-age)) age--;

        if (age > 0) return age == 1 ? "1 Year old" : $"{age} Years old";

        var months = (today.Year - birthDate.Year) * 12 + today.Month - birthDate.Month;
        if (birthDate.Day > today.Day) months--;

        return months <= 0 ? "Newborn" : (months == 1 ? "1 Month old" : $"{months} Months old");
    }

    protected void NextImg()
    {
        if (AnimalItem?.ImagePaths?.Count > 1)
            currentImageIndex = (currentImageIndex + 1) % AnimalItem.ImagePaths.Count;
    }

    protected void PrevImg()
    {
        if (AnimalItem?.ImagePaths?.Count > 1)
            currentImageIndex = (currentImageIndex - 1 + AnimalItem.ImagePaths.Count) % AnimalItem.ImagePaths.Count;
    }
}