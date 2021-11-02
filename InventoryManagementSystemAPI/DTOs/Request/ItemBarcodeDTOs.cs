using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class FindBarcodeItemDTO
    {
        [Required]
        public string Barcode { get; set; }

        public bool IsLoan { get; set; }
    }

    public class AddBarcodeItemDTO
    {
        [Required]
        public int ItemId { get; set; }

        [Required]
        public string Barcode { get; set; }

        public bool IsLoanItem { get; set; }

        public int Amount { get; set; }
    }

    public class RemoveBarcodeFromItemDTO
    {
        [Required]
        public string Barcode { get; set; }

        public bool IsConsumptionItem { get; set; }

        public int Amount { get; set; }
    }
}
