using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class ImageResponseDTO
    {
        public int ImageId { get; set; }

        public string ImageName { get; set; }

        public Uri ImageUri { get; set; }
    }

    public class ImageWithIsUsedResponseDTO
    {
        public int ImageId { get; set; }

        public string ImageName { get; set; }

        public Uri ImageUri { get; set; }
        public bool IsUsed { get; set; }
    }
}
