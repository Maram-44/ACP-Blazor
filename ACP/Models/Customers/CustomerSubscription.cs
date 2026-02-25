namespace ACP.Models.Customers
{
    public class CustomerSubscription
    {
        public int? Id { get; set; }
        public int SubscriptionId { get; set; }
        public int CustomerId { get; set; }

        public DateTime SubscriptionStartDate { get; set; }
        public DateTime? SubscriptionEndDate { get; set; }
    }
}
