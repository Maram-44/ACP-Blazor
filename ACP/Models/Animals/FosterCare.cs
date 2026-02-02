

namespace ACP.Models.Animals
{
    public class FosterCare
    {
        public int? FosterCareId { get; set; }
        public int CustomerId { get; set; }
        public int AnimalId { get; set; }
        public DateTime ArrivalDate { get; set; }
        public DateTime WithdrawalDate { get; set; }
        public decimal? Cost { get; set; }
    }
}
