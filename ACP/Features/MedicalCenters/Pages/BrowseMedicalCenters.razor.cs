using Microsoft.AspNetCore.Components;
using ACP.Features.MedicalCenters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using ACP.Features.MedicalCenters.DTOs;

namespace ACP.Features.MedicalCenters
{

    public partial class BrowseMedicalCenters : ComponentBase
    {
        // حقن الخدمة الحقيقية للباك آند
        [Inject] private MedicalCenterService MedicalService { get; set; } = default!;
        [Inject] private NavigationManager Navigation { get; set; } = default!;

        private List<MedicalCenter> CentersList = new();
        private List<MedicalCenter> FilteredCenters = new();
        private List<string> AvailableCities = new();
        private bool IsLoading = true;

        private string selectedCity = "all";

        protected override async Task OnInitializedAsync()
        {
            try
            {
                IsLoading = true;

                // جلب البيانات الحقيقية من السيرفر
                CentersList = await MedicalService.GetAll();

                // استخراج المدن المتوفرة ديناميكياً لتغذية الفلتر
                AvailableCities = CentersList
                    .Where(c => !string.IsNullOrEmpty(c.City))
                    .Select(c => c.City)
                    .Distinct()
                    .ToList();

                ApplyFilter();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching medical centers: {ex.Message}");
            }
            finally
            {
                IsLoading = false;
            }
        }

        // 🔹 الفلترة أصبحت مقتصرة على المدينة فقط بناءً على رغبتكِ
        private void ApplyFilter()
        {
            if (CentersList == null) return;

            // البدء دائماً بالقائمة الكاملة المجلوبة من الباك إند
            var query = CentersList.AsEnumerable();

            // الفلترة حسب المدينة (إذا تم اختيار مدينة معينة)
            if (!string.IsNullOrEmpty(selectedCity) && selectedCity != "all")
            {
                query = query.Where(c => c.City != null && c.City.Equals(selectedCity, StringComparison.OrdinalIgnoreCase));
            }

            // تحديث القائمة المعروضة على الشاشة
            FilteredCenters = query.ToList();

            // إجبار واجهة Blazor على إعادة الرسم الفوري
            StateHasChanged();
        }

        private void HandleFilterChanged((string city, string service) filter)
        {
            // نستقبل المدينة فقط ونمررها، ونتجاهل الخدمات
            selectedCity = filter.city;
            ApplyFilter();
        }

        private void ResetFilters()
        {
            selectedCity = "all";
            ApplyFilter();
        }

        private void NavigateToDetails(int id)
        {
            Navigation.NavigateTo($"/clinic-details/{id}");
        }
    }
}