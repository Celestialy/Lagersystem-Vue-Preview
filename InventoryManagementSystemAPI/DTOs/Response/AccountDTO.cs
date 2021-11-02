using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class AccountDTO
    {
        public string UserId { get; set; }

        public string Username { get; set; }

        public string DepartmentName { get; set; }

        public List<string> RoleNames { get; set; }
    }
}
