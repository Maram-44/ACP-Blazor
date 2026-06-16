using ACP.Models.Animals;
using ACP.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace ACP.Pages
{
    public partial class ListOfRequests
    {
        [Inject] public AnimalTransactionClientService TransactionService { get; set; } = default!;
        [Inject] public IJSRuntime JSRuntime { get; set; } = default!;

        // استخدام MyApplicationDTO بناءً على السيرفس
        protected List<MyApplicationDTO> AdoptionRequests { get; set; } = new();
        protected List<MyApplicationDTO> FosterRequests { get; set; } = new();
        
        protected bool IsLoading { get; set; } = true;
        protected string ActiveTab { get; set; } = "Adoption";

        protected string? PageMessage { get; set; }
        protected bool IsErrorMessage { get; set; } = false;
        protected HashSet<int> ConfirmingRequestIds { get; set; } = new();
        protected Dictionary<int, string> InputtedConfirmationCodes { get; set; } = new();

        protected override async Task OnAfterRenderAsync(bool firstRender)
        {
            if (firstRender)
            {
                await LoadApplications();
                IsLoading = false;
                StateHasChanged();
            }
        }

        private async Task LoadApplications()
        {
            try
            {
                var response = await TransactionService.GetMyApplicationsAsync();

                if (response != null)
                {
                    // الفصل بناءً على حقل الـ Status المتاح في المودل الأصلي لصفحتكِ
                    AdoptionRequests = response.Where(a => a.Status == "Adoption").ToList();
                    FosterRequests = response.Where(a => a.Status == "Foster").ToList();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching data: {ex.Message}");
            }
        }

        protected void ChangeTab(string tabName)
        {
            ActiveTab = tabName;
            PageMessage = null; 
        }

        protected async Task HandleConfirmDelivery(int applicationId)
        {
            if (!InputtedConfirmationCodes.TryGetValue(applicationId, out var code) || string.IsNullOrWhiteSpace(code) || code.Length < 6)
            {
                PageMessage = "Validation Failed: Please enter a valid 6-digit handover code.";
                IsErrorMessage = true;
                return;
            }

            try
            {
                ConfirmingRequestIds.Add(applicationId);
                PageMessage = null;

                // إنشاء الـ Request المقبول من السيرفس وتعيين القيم المتاحة فيه
                var confirmRequest = new ConfirmCodeRequest
                {
                    // ملاحظة: إذا كان الحقل داخل المودل يسبب خطأ باسم ApplicationId، جربي تغييره إلى Id بناءً على تعريف الكلاس لديكِ
                    TransactionId = applicationId, 
                    Code = code
                };

                var result = await TransactionService.ConfirmDeliveryAsync(confirmRequest);

                // فحص ما إذا كان الرد ليس نال (لأن السيرفس ترجع الـ Content عند النجاح)
                if (result != null)
                {
                    PageMessage = "Success! Handover confirmed. Welcome to your new family! 🎉";
                    IsErrorMessage = false;

                    // تحديث القائمة فوراً لحذف العنصر المستلم
                    AdoptionRequests.RemoveAll(a => a.ApplicationId == applicationId);
                    FosterRequests.RemoveAll(f => f.ApplicationId == applicationId);
                    InputtedConfirmationCodes.Remove(applicationId);
                }
                else
                {
                    PageMessage = "Failed to verify the code. Please try again.";
                    IsErrorMessage = true;
                }
            }
            catch (Exception ex)
            {
                PageMessage = $"An error occurred: {ex.Message}";
                IsErrorMessage = true;
            }
            finally
            {
                ConfirmingRequestIds.Remove(applicationId);
            }
        }
    }
}