using Microsoft.AspNetCore.Components;

namespace ACP.Components.MedicalCenters;

public partial class MedicalCenterCard
{
    [Parameter] public string Title { get; set; } = string.Empty;
    [Parameter] public string Description { get; set; } = string.Empty; // سنمرر هنا اسم الخدمة
    [Parameter] public string ImageUrl { get; set; } = string.Empty;
    [Parameter] public string Location { get; set; } = string.Empty;
    [Parameter] public string PhoneNumber { get; set; } = string.Empty; // خاصية الهاتف الجديدة
    [Parameter] public string Email { get; set; } = string.Empty;
}