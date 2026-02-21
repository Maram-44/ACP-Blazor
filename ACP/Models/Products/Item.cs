
using ACP.Application.DTOs.Products;

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
        public ItemsCategory Category { get; set; }=null!;
        public int? OwnerId { get; set; }

        public double AverageRating { get; set; }
        public int TotalReviews { get; set; }

        public ICollection<ItemPhoto> itemPhoto { get; set; }
        public ICollection<ProductComment> ProductComments { get; set; } = new List<ProductComment>();

    }
}
