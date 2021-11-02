using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{

    public class InventoryResponseDTO
    {
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public string InventoryType { get; set; }
    }

    public class InventoryWithDepartmentResponseDTO
    {
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public string InventoryType { get; set; }

        public DepartmentResponseDTO Department { get; set; }
    }

    public class InventoryWithDepartmentAndUsersResponseDTO
    {
        public int InventoryId { get; set; }

        public string Name { get; set; }

        public string Address { get; set; }

        public string Zipcode { get; set; }

        public string City { get; set; }

        public string InventoryType { get; set; }

        public DepartmentWithUsersResponseDTO Department { get; set; }
    }

}
