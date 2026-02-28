using Microsoft.AspNetCore.Components;
using ACP.Models.MedicalCenters;
using ACP.Services;



namespace ACP.Pages
{
    public partial class ClinicDetailsBase : ComponentBase
    {
        [Inject] public ACP.Services.MedicalCenterService MedicalService { get; set; } = default!;
        [Inject] public NavigationManager Navigation { get; set; } = default!;

       
        [Parameter] public int Id { get; set; }

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
            catch (Exception)
            {
                currentCenter = null;
            }
            finally
            {
                isLoading = false;
            }
        }

        protected void NextImage()
        {
            if (currentCenter?.medicalCenterServices != null && currentCenter.medicalCenterServices.Any())
            {
                if (currentImageIndex < currentCenter.medicalCenterServices.Count - 1)
                    currentImageIndex++;
                else
                    currentImageIndex = 0;
            }
        }

        protected void PrevImage()
        {
            if (currentCenter?.medicalCenterServices != null && currentCenter.medicalCenterServices.Any())
            {
                if (currentImageIndex > 0)
                    currentImageIndex--;
                else
                    currentImageIndex = currentCenter.medicalCenterServices.Count - 1;
            }
        }
    }
}