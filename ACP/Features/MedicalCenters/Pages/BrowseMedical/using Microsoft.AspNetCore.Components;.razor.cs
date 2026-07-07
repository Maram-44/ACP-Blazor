using Microsoft.AspNetCore.Components;

namespace ACP.Components.MedicalCenters;

public partial class MedicalFilter
{
    private string SelectedCity { get; set; } = "all";
    private string SelectedService { get; set; } = "all";

    // نمرر قائمة المدن من الصفحة الرئيسية ليكون الفلتر مرن
    [Parameter] public List<string>? Cities { get; set; }

    [Parameter] public EventCallback<(string city, string service)> OnFilterChanged { get; set; }

    private async Task SetService(string service)
    {
        SelectedService = service;
        await AutoApply();
    }

    private async Task AutoApply()
    {
        if (OnFilterChanged.HasDelegate)
        {
            await OnFilterChanged.InvokeAsync((SelectedCity, SelectedService));
        }
    }

    protected override async Task OnAfterRenderAsync(bool firstRender)
    {
        if (firstRender)
        {
            await AutoApply();
        }
    }
}