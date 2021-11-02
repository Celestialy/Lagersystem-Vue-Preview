using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class CategoryResponseDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }
    }

    public class CategoryWithIsUsedResponseDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }
        public bool IsUsed { get; set; }
    }


    public class CategoryWithLoanItemsResponseDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public List<LoanItemResponseDTO> LoanItems { get; set; }
    }

    public class CategoryWithConsumptionItemsResponseDTO
    {
        public int CategoryID { get; set; }

        public string CategoryName { get; set; }

        public List<ConsumptionItemResponseDTO> ConsumptionItemResponses { get; set; }
    }
}
