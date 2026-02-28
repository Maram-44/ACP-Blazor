using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace ACP.Models.Customers
{
    public class Subscription
    {
        public int? SubscriptionId { get; set; }

        [Required]
        [MaxLength(100)]
        public string SubscriptionName { get; set; } = null!;

        [Required]
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        [Required]
        public int PeriodInDays { get; set; }

        [Required]
        public int QTYItems { get; set; }
    }
}
