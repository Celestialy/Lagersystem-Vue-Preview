using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class BasicItemResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public int Amount { get; set; }

        public string Category { get; set; }

        public string ImageUri { get; set; }

        public bool IsAvailable { get; set; }
    }

    #region Loan
    public class UserLoanItemResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public CategoryResponseDTO Category { get; set; }

        public ImageResponseDTO Image { get; set; }

        public InventoryResponseDTO Inventory { get; set; }

        public bool IsAvailable { get; set; }

        public string CreatedAt { get; set; }

        public string UpdatedAt { get; set; }
    }
    public class LoanItemResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public List<string> Barcodes { get; set; }

        public ImageResponseDTO Image { get; set; }

        public InventoryResponseDTO Inventory { get; set; }

        public bool IsAvailable { get; set; }
    }

    public class LoanItemWithCategoryResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public CategoryResponseDTO Category { get; set; }

        public List<string> Barcodes { get; set; }

        public ImageResponseDTO Image { get; set; }

        public InventoryResponseDTO Inventory { get; set; }

        public int AmountLeft { get; set; }
    }
    #endregion

    #region Consumption
    public class UserConsumptionItemResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public CategoryResponseDTO Category { get; set; }

        public ImageResponseDTO Image { get; set; }

        public int Amount { get; set; }

        public InventoryResponseDTO Inventory { get; set; }

        public DateTime Date { get; set; }
    }

    public class ConsumptionItemWithCategoryResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public CategoryResponseDTO Category { get; set; }

        public List<string> Barcodes { get; set; }

        public ImageResponseDTO Image { get; set; }

        public int AmountLeft { get; set; }

        public InventoryResponseDTO Inventory { get; set; }
    }

    public class ConsumptionItemResponseDTO
    {
        public int ItemId { get; set; }

        public string Brand { get; set; }

        public string Model { get; set; }

        public string Description { get; set; }

        public List<string> Barcodes { get; set; }

        public ImageResponseDTO Image { get; set; }

        public int AmountLeft { get; set; }

        public InventoryResponseDTO Inventory { get; set; }
    }
    #endregion

}
