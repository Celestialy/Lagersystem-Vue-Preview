using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{

    public class UserConsumptionResponseDTO
    {
        public UserResponseDTO User { get; set; }

        public UserConsumptionItemResponseDTO UserConsumptionItem { get; set; }
    }

    public class UserConsumptionHistoryResponseDTO
    {
        public UserResponseDTO User { get; set; }

        public List<UserConsumptionItemResponseDTO> ConsumptionItems { get; set; }
    }
    public class GetAllUserConsumptionHistoryResponseDTO
    {
        public BasicUserResponseDTO User { get; set; }

        public BasicItemResponseDTO ConsumptionItem { get; set; }
        public string ConsumptionDate { get; set; }
    }
}
