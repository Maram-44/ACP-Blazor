using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Models.Products
{
    public class ItemPhoto
    {
        public int? PhotoId { get; set; }
        public string PhotoName { get; set; } = null!;
        public bool IsMainPhoto { get; set; }
        public int ItemId { get; set; }
    }
}
