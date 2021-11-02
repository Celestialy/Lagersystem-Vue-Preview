using InventoryManagementSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class GetConsumptionItemsDTO
    {
        [Required]
        public int InventoryId { get; set; }
    }
    public class GetConsumptionItemDTO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public int ItemId { get; set; }
    }

    public class AddConsumptionItemDTO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ImageId { get; set; }
    }

    public class EditConsumptionItemDTO
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        [Required]
        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int AmountLeft { get; set; }

        [Required]
        public int ImageId { get; set; }
    }
    public class GetUserConsumptionDTO
    {
        public string UserId { get; set; }

        [Required]
        public int ItemId { get; set; }
    }

    public class AddUserConsumptionDTO
    {
        public string UserId { get; set; }

        [Required]
        public string ItemBarcode { get; set; }

        [Required]
        public int ItemAmount { get; set; }
    }
}