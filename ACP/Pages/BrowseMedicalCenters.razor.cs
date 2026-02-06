using Microsoft.AspNetCore.Components;
using ACP.Models.MedicalCenters; // الموديل الحقيقي من الباك آند
using ACP.Services;             // الخدمة الحقيقية

namespace ACP.Pages;

public partial class BrowseMedicalCenters
{
    // حقن الخدمة التي برمجها زميلك
    [Inject] private ACP.Services.MedicalCenterService MedicalService { get; set; } = default!;
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private List<MedicalCenter> CentersList = new();
    private List<MedicalCenter> FilteredCenters = new();
    private List<string> AvailableCities = new();
    private bool IsLoading = true;

    private string selectedCity = "all";
    private string selectedService = "all";

    protected override async Task OnInitializedAsync()
    {
        try
        {
            IsLoading = true;
            // استدعاء البيانات الحقيقية من الـ API
            CentersList = await MedicalService.GetAll();

            // استخراج المدن المتوفرة ديناميكياً من البيانات
            AvailableCities = CentersList
                .Where(c => !string.IsNullOrEmpty(c.City))
                .Select(c => c.City)
                .Distinct()
                .ToList();

            ApplyFilter();
        }
        catch (Exception ex)
        {
            // يمكنك إضافة نظام تنبيه هنا في حال فشل السيرفر
            Console.WriteLine($"Error fetching medical centers: {ex.Message}");
        }
        finally
        {
            IsLoading = false;
        }
    }

    private void ApplyFilter()
    {
        FilteredCenters = CentersList.Where(c =>
            // 1. فلترة المدينة
            (selectedCity == "all" || c.City == selectedCity) &&

            // 2. فلترة الخدمة (نبحث داخل قائمة الخدمات التابعة للمركز)
            (selectedService == "all" || (c.medicalCenterServices != null &&
             c.medicalCenterServices.Any(s => s.ServiceName.Contains(selectedService, StringComparison.OrdinalIgnoreCase))))
        ).ToList();

        StateHasChanged();
    }

    private void HandleFilterChanged((string city, string service) filter)
    {
        selectedCity = filter.city;
        selectedService = filter.service;
        ApplyFilter();
    }

    private void ResetFilters()
    {
        selectedCity = "all";
        selectedService = "all";
        ApplyFilter();
    }

    private void NavigateToDetails(int id)
    {
        Navigation.NavigateTo($"/medical-center-details/{id}");
    }
}