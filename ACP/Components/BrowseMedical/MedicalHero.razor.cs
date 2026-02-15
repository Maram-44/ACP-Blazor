using Microsoft.AspNetCore.Components;

namespace ACP.Components.MedicalCenters;

public partial class MedicalHero
{
    [Parameter]
    public string BadgeText { get; set; } = "We care for your best friend ✨";

    [Parameter]
    public string TitlePart1 { get; set; } = "Premium Care";

    [Parameter]
    public string TitlePart2 { get; set; } = "For Your Pets";

    [Parameter]
    public string Description { get; set; } = "From advanced medical checkups to grooming sessions, we connect you with the best experts in your area at the click of a button.";

    [Parameter]
    public EventCallback OnBookingClick { get; set; }

    private async Task HandleBooking()
    {
        if (OnBookingClick.HasDelegate)
        {
            await OnBookingClick.InvokeAsync();
        }
    }
}