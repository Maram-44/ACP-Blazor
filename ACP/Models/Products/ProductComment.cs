using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Application.DTOs.Products
{
    public class ProductComment
    {
        public int? Id { get; set; }
        [MaxLength(200)]
        public string Comment { get; set; } = null!;
        public int CustomerId { get; set; }
    }
}
