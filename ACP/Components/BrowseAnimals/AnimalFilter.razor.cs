using Microsoft.AspNetCore.Components;
using ACP.Models.Animals;
using ACP.Services; // تأكدي من إضافة هذا المسار للوصول لموديل AnimalType

namespace ACP.Components.BrowseAnimals;

public partial class AnimalFilter
{
    [Parameter] public string SearchText { get; set; } = "";
    [Parameter] public int SelectedTypeId { get; set; } = 0;
    [Parameter] public string SelectedAgeRange { get; set; } = "all";

    // الأضافة الجديدة: استقبال قائمة الأنواع من صفحة BrowseAnimals
    [Parameter] public List<AnimalType>? AnimalTypes { get; set; }

    [Parameter] public EventCallback<string> OnSearchChanged { get; set; }
    [Parameter] public EventCallback<int> OnTypeChanged { get; set; }
    [Parameter] public EventCallback<string> OnAgeChanged { get; set; }



    private async Task OnSearchInput(ChangeEventArgs e)
    {
        SearchText = e.Value?.ToString() ?? "";
        await OnSearchChanged.InvokeAsync(SearchText);
    }

    private async Task OnTypeChange(ChangeEventArgs e)
    {
        if (int.TryParse(e.Value?.ToString(), out var id))
        {
            SelectedTypeId = id;
            await OnTypeChanged.InvokeAsync(SelectedTypeId);
        }
    }

    private async Task OnAgeChange(ChangeEventArgs e)
    {
        SelectedAgeRange = e.Value?.ToString() ?? "all";
        await OnAgeChanged.InvokeAsync(SelectedAgeRange);
    }
}