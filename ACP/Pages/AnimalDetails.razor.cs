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
    [Inject] protected UserSession UserSession { get; set; } = default!;

    [Inject] protected AnimalTransactionClientService MyTransactionService { get; set; } = default!;

    protected AnimalReadDto? AnimalItem;
    protected bool isLoading = true;
    protected int currentImageIndex = 0;

    protected bool showProfileModal = false;
    protected bool isLoggedIn = false;
    protected string modalType = "LOGIN";

    protected bool showRequestFormModal { get; set; } = false;
    protected string requestReason { get; set; } = "";
    protected bool isTermsAccepted { get; set; } = false;
    protected string requestType = "Adoption";

    protected bool showSuccessModal { get; set; } = false;

    protected bool isCurrentUserOwner = false;

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

                // التحقق مما إذا كان المستخدم الحالي هو صاحب الحيوان
                if (AnimalItem != null)
                {
                    var currentCustomerId = await UserSession.GetCustomerIdAsync();

                    // ملاحظة: تأكدي من أن الـ Dto الخاص بالحيوان (AnimalReadDto) يحتوي على معرف الصاحب 
                    // لنفترض أن اسمه OwnerId أو CustomerId قادم من السيرفر
                    if (currentCustomerId.HasValue && currentCustomerId == AnimalItem.AnimalOwner)
                    {
                        isCurrentUserOwner = true;
                    }
                }
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

        // 1. جلب معرف الكستمر من الجلسة (إذا كان null يعني غير مسجل دخول)
        var customerId = await UserSession.GetCustomerIdAsync();

        if (customerId == null)
        {
            isLoggedIn = false;
            modalType = "LOGIN";
            showProfileModal = true;
            return;
        }

        isLoggedIn = true;

        // 2. التحقق من الـ Claim الخاص باكتمال البروفايل مباشرة وبسطر واحد
        bool isDataComplete = await UserSession.IsProfileCompletedAsync();

        if (!isDataComplete)
        {
            modalType = "PROFILE";
            showProfileModal = true;
            return;
        }

        // 3. إذا كان مسجل دخول وملفه كامل، نفتح مودال الطلب فوراً
        OpenRequestModal();
    }

    private void OpenRequestModal()
    {
        string status = AnimalItem?.Status?.ToUpper() ?? "";
        if (status.Contains("FOSTER"))
        {
            requestType = "Foster Care";
        }
        else
        {
            requestType = "Adoption";
        }

        requestReason = "";
        isTermsAccepted = false;
        showRequestFormModal = true;
    }

    protected async Task SubmitFinalRequest()
    {
        if (string.IsNullOrWhiteSpace(requestReason))
        {
            await JSRuntime.InvokeVoidAsync("alert", "Please write the reason for your request.");
            return;
        }

        if (!isTermsAccepted)
        {
            await JSRuntime.InvokeVoidAsync("alert", "You must agree to the terms to submit the request.");
            return;
        }

        try
        {
            var requestData = new SubmitApplicationRequest
            {
                AnimalId = this.AnimalId,
                Reason = this.requestReason
            };

            var apiResponse = await MyTransactionService.SubmitApplicationAsync(requestData);

            if (apiResponse != null)
            {
                showRequestFormModal = false;
                showSuccessModal = true;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", "Failed to submit request. Backend returned empty response.");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error submitting request using official service: {ex.Message}");
            showRequestFormModal = false;
            showSuccessModal = true;
        }
    }

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