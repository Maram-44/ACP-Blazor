using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using Microsoft.AspNetCore.Components.Authorization;
using ACP.Models.Customers;
using ACP.Services;
using Microsoft.JSInterop;
using System.Security.Claims;

namespace ACP.Pages;

public partial class ProfilePage : ComponentBase
{
    [Inject] protected CustomerService MyCustomerService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected AuthenticationStateProvider AuthProvider { get; set; } = default!;

    protected CustomerProfile? userProfile;
    protected bool isLoading = true;
    protected bool isSaving = false;
    protected bool showSuccessModal = false;
    protected string? imagePreview;

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;
            var authState = await AuthProvider.GetAuthenticationStateAsync();
            var user = authState.User;

            if (user.Identity is not { IsAuthenticated: true })
            {
                Navigation.NavigateTo("SignUp");
                return;
            }

            var userIdClaim = user.FindFirst(ClaimTypes.NameIdentifier)?.Value
                            ?? user.FindFirst("sub")?.Value
                            ?? user.FindFirst("uid")?.Value
                            ?? user.FindFirst("nameid")?.Value;

            int currentUserId = int.TryParse(userIdClaim, out var id) ? id : 0;

            userProfile = await MyCustomerService.GetProfileAsync();

            if (userProfile == null)
            {
                userProfile = new CustomerProfile
                {
                    UserId = currentUserId,
                    Email = CleanEmail(user.FindFirst(ClaimTypes.Email)?.Value ?? ""),
                    FirstName = user.FindFirst("FirstName")?.Value ?? "",
                    LastName = user.FindFirst("LastName")?.Value ?? "",
                    UserName = user.Identity.Name ?? "User",
                    MiddleName = user.FindFirst("MiddleName")?.Value ?? "",
                    Nationality = "",
                    PhoneNumber = "",
                    TypeOfIdentity = "",
                    IdentityNumber = ""
                };
            }
            else
            {
                if (userProfile.UserId == 0) userProfile.UserId = currentUserId;
                userProfile.Email = CleanEmail(userProfile.Email);
                imagePreview = userProfile.ImageWithIdentity;
            }
        }
        catch (Exception ex) { Console.WriteLine($"Init Error: {ex.Message}"); }
        finally { isLoading = false; }
    }

    private string CleanEmail(string email)
    {
        if (string.IsNullOrEmpty(email)) return "";
        if (email.Contains("["))
        {
            var parts = email.Split(new[] { '"', ',', '[', ']' }, StringSplitOptions.RemoveEmptyEntries);
            return parts.FirstOrDefault(p => p.Contains("@"))?.Trim() ?? email;
        }
        return email.Trim();
    }

    protected async Task HandleUpdate()
    {
        if (userProfile == null) return;

        userProfile.MiddleName ??= "";
        userProfile.PhoneNumber ??= "";
        userProfile.TypeOfIdentity ??= "";
        userProfile.Nationality ??= "";

        if (!string.IsNullOrEmpty(imagePreview))
        {
            userProfile.ImageWithIdentity = imagePreview;
        }

        isSaving = true;
        try
        {
            var result = await MyCustomerService.UpdateProfileAsync(userProfile);

            if (result == "Success")
            {
                showSuccessModal = true;
                var freshProfile = await MyCustomerService.GetProfileAsync();
                if (freshProfile != null) userProfile = freshProfile;
            }
            else
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Server Error: {result}");
            }
        }
        catch (Exception ex)
        {
            await JSRuntime.InvokeVoidAsync("alert", "Connection failed.");
            Console.WriteLine(ex.Message);
        }
        finally { isSaving = false; }
    }

    protected void CloseSuccessModal()
    {
        showSuccessModal = false;
        StateHasChanged();
    }

    protected async Task HandleImageUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            // تم تكبير حجم المعاينة لتناسب الأبعاد الجديدة للهوية 
            var resizedFile = await file.RequestImageFileAsync("image/png", 600, 400);
            var buffer = new byte[resizedFile.Size];
            using var stream = resizedFile.OpenReadStream(maxAllowedSize: 2 * 1024 * 1024);
            await stream.ReadAsync(buffer);

            imagePreview = $"data:image/png;base64,{Convert.ToBase64String(buffer)}";

            if (userProfile != null)
            {
                userProfile.ImageWithIdentity = imagePreview;
            }
        }
    }
}