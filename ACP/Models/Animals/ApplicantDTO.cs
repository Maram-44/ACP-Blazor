namespace ACP.Models.Animals
{
    public class ApplicantDTO
    {
        public int TransactionId { get; set; }
        public int CustomerId { get; set; }
        public string FullName { get; set; }
        public string Nationality { get; set; }
        public string ReasonOfApplication { get; set; }
    }
}
