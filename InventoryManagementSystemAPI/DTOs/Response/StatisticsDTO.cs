using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs.Response
{
    public class InventoryStatisticsDTO
    {
        public int LoanAmount { get; set; }
        public int LoanedAmount { get; set; }

        public int ConsumptionAmount { get; set; }
    }

    public class LowItemAmountDTO
    {
        public string Brand { get; set; }

        public string Model { get; set; }

        public string Category { get; set; }

        public Uri ImageUri { get; set; }

        public int AmountLeft { get; set; }
        
        public int TotalAmount { get; set; }
    }
}