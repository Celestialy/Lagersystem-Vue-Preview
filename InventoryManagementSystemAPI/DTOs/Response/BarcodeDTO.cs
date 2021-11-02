using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class ItemBarcodeResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public string Category { get; set; }

        public ImageResponseDTO Image { get; set; }

        public int AmountLeft { get; set; }
    }
}
