using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Application.DTOs.Products
{
    public class ItemDTO
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

        public ICollection<ItemPhotoDTO> itemPhoto { get; set; }

    }
}
