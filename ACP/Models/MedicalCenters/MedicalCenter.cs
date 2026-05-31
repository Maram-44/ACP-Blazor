

namespace ACP.Models.MedicalCenters
{
    public class MedicalCenter
    {
        public int? Id { get; set; }
        public string Name { get; set; } = null!;
        public string Country { get; set; } = null!;
        public string City { get; set; } = null!;
        public string Street { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string ImagePath { get; set; } = null!;
        public ICollection<MedicalCenterService>? medicalCenterServices { get; set; }
    }
}
