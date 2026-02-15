using ACP.Domain.Entities.Productes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ACP.Application.DTOs.Products
{
    public class ItemPhotoDTO
    {
        public int? PhotoId { get; set; }
        public string PhotoName { get; set; } = null!;
        public int ItemId { get; set; }
    }
}
