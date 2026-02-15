using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Models.Products
{
    public class Order
    {
        public int? OrderId { get; set; }
        public DateTime Date { get; set; }
        public string Status { get; set; } = null!;
        public int CustomerId { get; set; }
        // الحقول المحاسبية
        public decimal SubTotal { get; set; }
        public decimal Tax { get; set; }
        public decimal DeliveryFee { get; set; }
        public decimal TotalPrice { get; set; }
        public ICollection<OrderItem> orderItems { get; set; }
    }
}
