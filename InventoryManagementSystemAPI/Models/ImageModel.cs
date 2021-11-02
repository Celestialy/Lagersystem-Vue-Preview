using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class ImageModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string ImageName { get; set; }

        [Required]
        public Uri ImageUri { get; set; }

        [Required]
        public int DepartmentId { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}