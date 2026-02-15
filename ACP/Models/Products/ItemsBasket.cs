using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Models.Products
{
    public class ItemsBasket
    {
        public int? Id { get; set; }
        public int QTY { get; set; }
        public bool MarkForBuy { get; set; }

        public int CustomerId { get; set; }
        public int ItemId { get; set; }
    }
}
