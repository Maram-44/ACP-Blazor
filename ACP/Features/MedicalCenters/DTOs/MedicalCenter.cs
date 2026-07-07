

namespace ACP.Features.MedicalCenters.DTOs
{
    public class MedicalCenter
    {
        public int Id { get; set; }
        public string Name { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public ICollection<MedicalCenterServices>? medicalCenterServices { get; set; }
    }
}
