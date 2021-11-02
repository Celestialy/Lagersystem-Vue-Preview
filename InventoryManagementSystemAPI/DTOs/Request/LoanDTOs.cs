using InventoryManagementSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class GetLoanItemsDTO
    {
        [Required]
        public int InventoryId { get; set; }

        public bool ShowAvailableItems { get; set; }
    }

    public class GetLoanItemDTO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string ItemBarcode { get; set; }
    }

    public class AddLoanItemDTO
    {
        [Required]
        public int InventoryId { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        public string Description { get; set; }

        [Required]
        public int CategoryId { get; set; }

        [Required]
        public int ImageId { get; set; }
    }

    public class EditLoanItemDTO
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
        public int ImageId { get; set; }
    }

    public class DeleteLoanItemDTO
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public int InventoryId { get; set; }
    }

    public class GetUserLoansDTO
    {
        public string UserId { get; set; }

        public bool ShowAvailableItems { get; set; }
    }

    public class GetUserLoanDTO
    {
        [Required]
        public string ItemBarcode { get; set; }
    }

    public class GetUserLoanHistory
    {
        public bool ShowHandedInItems { get; set; }
    }

    public class AddUserLoanDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ItemBarcode { get; set; }
    }

    public class DeleteUserLoanDTO
    {
        [Required]
        public string UserId { get; set; }

        [Required]
        public string ItemBarcode { get; set; }
    }

    public class FindUserLoansByBarcode
    {
        [Required]
        public string Barcode { get; set; }
    }
}