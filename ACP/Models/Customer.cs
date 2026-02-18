using System.ComponentModel.DataAnnotations;

namespace ACP.Models
{
    public class Customer
    {
        public int? CustomerId { get; set; }
        public string FirstName { get; set; } = null!;
        public string MiddleName { get; set; } = null!;
        public string LastName { get; set; } = null!;
        public string PhoneNumber { get; set; } = null!;
        public string Email { get; set; } = null!;

        public string IdentityNumber { get; set; }
        public string TypeOfIdentity { get; set; }
        public string Nationality { get; set; }

        public string? ImageWithIdentity { get; set; }
        public bool? ConfermImageWithIdentity { get; set; }
        public bool IsCompleteProfile { get; set; }

        public int UserId { get; set; }
    }
}
