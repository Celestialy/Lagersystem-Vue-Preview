using InventoryManagementSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class AddInventoryDTO
    {
        public int InventoryId { get; set; }

        [Required]
        public string InventoryName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string InventoryType { get; set; }

        public DateTime CreatedAt { get; set; }
    }

    public class EditInventoryDTO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string InventoryName { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string ZipCode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string InventoryType { get; set; }

        public DateTime UpdatedAt { get; set; }
    }

    public class DeleteInventoryDTO
    {
        [Required]
        public int InventoryId { get; set; }
    }
}