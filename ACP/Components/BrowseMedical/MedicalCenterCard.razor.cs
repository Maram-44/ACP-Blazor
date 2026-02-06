using Microsoft.AspNetCore.Components;

namespace ACP.Components.MedicalCenters;

public partial class MedicalCenterCard
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty;
    [Parameter] public string ImageUrl { get; set; } = "images/default-center.jpg";
    [Parameter] public string Location { get; set; } = "Unknown";

    // هذا المتغير هو المفتاح الآن
    [Parameter] public bool IsInstantBooking { get; set; } = true;

    // حدث عند الضغط على الكرت أو الزر
    [Parameter] public EventCallback OnViewDetails { get; set; }
    [Parameter] public EventCallback OnCallRequest { get; set; }

    private async Task HandleClick()
    {
        if (IsInstantBooking)
        {
            if (OnViewDetails.HasDelegate)
                await OnViewDetails.InvokeAsync();
        }
        else
        {
            // هنا يمكن تنفيذ منطق إظهار رقم الهاتف
            if (OnCallRequest.HasDelegate)
                await OnCallRequest.InvokeAsync();
        }
    }
}