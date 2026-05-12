using Microsoft.AspNetCore.Components;
using ACP.Models.MedicalCenters;
using ACP.Services;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace ACP.Pages
{
    public partial class ClinicDetailsBase : ComponentBase
    {
        [Inject]
        public ACP.Services. MedicalCenterService MedicalService { get; set; } = default!;

        [Inject]
        public NavigationManager Navigation { get; set; } = default!;

        [Parameter]
        public int Id { get; set; }

        protected int currentImageIndex = 0;
        protected MedicalCenter? currentCenter;
        protected bool isLoading = true;

        protected override async Task OnInitializedAsync()
        {
            isLoading = true;
            try
            {
                if (Id > 0)
                {
                    currentCenter = await MedicalService.GetById(Id);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching clinic: {ex.Message}");
                currentCenter = null;
            }
            finally
            {
                isLoading = false;
            }
        }

        // منطق فصل الاسم ديناميكياً
        protected (string grayPart, string orangePart) SplitClinicName(string fullName)
        {
            if (string.IsNullOrWhiteSpace(fullName)) return ("", "");

            var words = fullName.Split(new[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

            // تلوين أول كلمتين بالرمادي والباقي بالبرتقالي
            if (words.Length <= 2)
                return (fullName, "");

            string gray = string.Join(" ", words.Take(2));
            string orange = string.Join(" ", words.Skip(2));

            return (gray, orange);
        }

        protected void SelectService(int index)
        {
            currentImageIndex = index;
            StateHasChanged();
        }

        protected void NextImage()
        {
            var services = currentCenter?.medicalCenterServices;
            if (services != null && services.Any())
            {
                currentImageIndex = (currentImageIndex + 1) % services.Count;
            }
        }

        protected void PrevImage()
        {
            var services = currentCenter?.medicalCenterServices;
            if (services != null && services.Any())
            {
                currentImageIndex = (currentImageIndex - 1 + services.Count) % services.Count;
            }
        }
    }
}