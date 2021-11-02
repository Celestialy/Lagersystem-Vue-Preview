using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class GetImageDTO
    {
        [Required]
        public int ImageId { get; set; }
    }

    public class GetImagesDTO
    {
        [Required]
        public List<int> ImageIdList { get; set; }
    }

    public class AddImageDTO
    {
        public string ImageName { get; set; }
    }


    public enum ImageModeDTO
    {
        standard = 0,
        WitchCheck = 1
    }
}