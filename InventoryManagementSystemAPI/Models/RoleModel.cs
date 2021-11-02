using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using Microsoft.AspNetCore.Identity;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class RoleModel : IdentityRole, IModel
    {
        public virtual ICollection<UserRoleModel> Users { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}