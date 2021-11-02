using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class DepartmentResponseDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }
    }

    public class DepartmentWithUsersAndInventoriesResponseDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public List<UserResponseDTO> Users { get; set; }

        public List<InventoryResponseDTO> Inventories { get; set; }
    }

    public class DepartmentWithUsersResponseDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public List<UserResponseDTO> Users { get; set; }
    }


    public class DepartmentWithUserConsumptionResponseDTO
    {
        public int DepartmentId { get; set; }

        public string DepartmentName { get; set; }

        public List<UserConsumptionHistoryResponseDTO> UserConsumptions { get; set; }
    }
}
