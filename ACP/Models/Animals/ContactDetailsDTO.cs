namespace ACP.Models.Animals
{
    public class ContactDetailsDTO
    {
        public string FullName { get; set; }
        public string PhoneNumber { get; set; }
        public string Email { get; set; }
        public int TransactionId { get; set; }
        public string StatusTransaction { get; set; } = string.Empty;
    }
}
