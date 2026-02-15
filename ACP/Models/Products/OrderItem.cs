using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Application.DTOs.Products
{
    public class OrderItemDTO
    {
        public int? Id { get; set; }
        public int QTY { get; set; }
        public decimal Price { get; set; }
        public int OrderId { get; set; }
        public int ItemId { get; set; }
    }
}
