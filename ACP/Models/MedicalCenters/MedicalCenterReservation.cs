

namespace ACP.Models.MedicalCenters
{
    public class MedicalCenterReservation
    {
        public int ReservationId { get; set; }
        public int MedicalCenterId { get; set; }
        public int CustomerId { get; set; }
        public DateTime Date { get; set; }
        public TimeSpan Time { get; set; }
    }
}
