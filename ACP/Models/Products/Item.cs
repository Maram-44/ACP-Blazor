
namespace ACP.Models.Products
{
    public class Item
    {
        public int? ItemId { get; set; }
        public string ItemName { get; set; } = null!;
        public string SKU { get; set; } = null!;
        public string Description { get; set; } = null!;
        public int QTY { get; set; }

        public decimal Cost { get; set; }
        public decimal SellPrice { get; set; }
        public decimal? DiscountRate { get; set; }
        public bool IsReturendable { get; set; }


        public int CategoryId { get; set; }
        public int? OwnerId { get; set; }

        public ICollection<ItemPhoto> itemPhoto { get; set; }

    }
}
