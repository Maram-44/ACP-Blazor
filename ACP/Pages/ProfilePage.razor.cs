using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;
using ACP.Models.Customers;
using ACP.Services;
using Microsoft.JSInterop;

namespace ACP.Pages;

public partial class ProfilePage : ComponentBase
{
    [Inject] protected CustomerService MyCustomerService { get; set; } = default!;
    [Inject] protected NavigationManager Navigation { get; set; } = default!;
    [Inject] protected IJSRuntime JSRuntime { get; set; } = default!;
    [Inject] protected UserSession Session { get; set; } = default!;

    protected CustomerProfile? userProfile;
    protected bool isLoading = true;
    protected bool isSaving = false;
    protected bool showSuccessModal = false;
    protected string? imagePreview; // سنستمر في استخدامه فقط كمعاينة بصرية للمستخدم بالصفحة

    protected override async Task OnInitializedAsync()
    {
        try
        {
            isLoading = true;

            int? currentCustomerId = await Session.GetCustomerIdAsync();

            if (currentCustomerId == null || currentCustomerId == 0)
            {
                Navigation.NavigateTo("logIn");
                return;
            }

            userProfile = await MyCustomerService.GetProfileAsync();

            if (userProfile == null)
            {
                userProfile = new CustomerProfile
                {
                    CustomerId = currentCustomerId.Value,
                    FirstName = "",
                    LastName = "",
                    MiddleName = "",
                    Email = "",
                    Nationality = "",
                    PhoneNumber = "",
                    TypeOfIdentity = "",
                    IdentityNumber = ""
                };
            }
            else
            {
                if (userProfile.CustomerId == 0)
                {
                    userProfile.CustomerId = currentCustomerId.Value;
                }

                // تأمين الحقول من الـ null
                userProfile.FirstName ??= "";
                userProfile.LastName ??= "";
                userProfile.MiddleName ??= "";
                userProfile.Nationality ??= "";
                userProfile.PhoneNumber ??= "";
                userProfile.TypeOfIdentity ??= "";
                userProfile.IdentityNumber ??= "";

                userProfile.Email = CleanEmail(userProfile.Email);

                // 👈 هنا التعديل: الباكيند الآن يعيد رابط كامل للصورة (ImagePath القادم من السيرفر)
                // نضعه في المعاينة مباشرة ليظهر للمستخدم فور فتح الصفحة
                imagePreview = userProfile.ImageWithIdentity;
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Init Profile Error: {ex.Message}");
        }
        finally
        {
            isLoading = false;
        }
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

        // فحص تأكيد كلمة المرور في الفرونتيند قبل إرسالها للسيرفر
        if (!string.IsNullOrEmpty(userProfile.NewPassword) && userProfile.NewPassword != userProfile.ConfirmNewPassword)
        {
            await JSRuntime.InvokeVoidAsync("alert", "New password and confirmation do not match.");
            return;
        }

        userProfile.MiddleName ??= "";
        userProfile.PhoneNumber ??= "";
        userProfile.TypeOfIdentity ??= "";
        userProfile.Nationality ??= "";

        isSaving = true;
        try
        {
            var result = await MyCustomerService.UpdateProfileAsync(userProfile);

            if (result == "Success")
            {
                showSuccessModal = true;

                // جلب البيانات مجدداً لتحديث الروابط والبيانات الطازجة من السيرفر
                var freshProfile = await MyCustomerService.GetProfileAsync();
                if (freshProfile != null)
                {
                    userProfile = freshProfile;
                    imagePreview = userProfile.ImageWithIdentity; // تحديث المعاينة بالرابط الجديد من السيرفر
                }
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

    // 👈 دالة الرفع المحدثة والجذرية لمعالجة الصورة كـ Files وبايتات حقيقية للـ Multipart
    protected async Task HandleImageUpload(InputFileChangeEventArgs e)
    {
        var file = e.File;
        if (file != null)
        {
            try
            {
                // 1. تقليص وتجهيز الصورة بأبعاد مناسبة للهوية
                var resizedFile = await file.RequestImageFileAsync("image/jpeg", 800, 600);

                // 2. قراءة مصفوفة البايتات وحفظها في الذاكرة
                var buffer = new byte[resizedFile.Size];
                using var stream = resizedFile.OpenReadStream(maxAllowedSize: 5 * 1024 * 1024); // حد أقصى 5 ميجا للرفع الآمن
                await stream.ReadAsync(buffer);

                if (userProfile != null)
                {
                    // 3. شحن الخصائص الجديدة داخل الـ DTO ليتم حزمها في الـ FormContent عند الحفظ
                    userProfile.ImageFileBytes = buffer;
                    userProfile.ImageFileName = file.Name;
                }

                // 4. إنشاء رابط محلي مؤقت (Base64) لكي يرى المستخدم الصورة فوراً في الصفحة كمعاينة بصرية قبل الضغط على حفظ
                imagePreview = $"data:image/jpeg;base64,{Convert.ToBase64String(buffer)}";
            }
            catch (Exception ex)
            {
                await JSRuntime.InvokeVoidAsync("alert", $"Image upload failed: {ex.Message}");
            }
        }
    }
}