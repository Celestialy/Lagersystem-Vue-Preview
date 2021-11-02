using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;

namespace InventoryManagementSystemAPI.Models
{
    public class UserRoleModel : IdentityUserRole<string>
    {
        private RoleModel _role;
        public UserRoleModel()
        {

        }
        private UserRoleModel(ILazyLoader lazyLoader)
        {
            _lazyloader = lazyLoader;
        }

        private ILazyLoader _lazyloader;
        public virtual UserModel User { get; set; }
        public RoleModel Role { get => _lazyloader.Load(this, ref _role); set => _role = value; }
    }
}
