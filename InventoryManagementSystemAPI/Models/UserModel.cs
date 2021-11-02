using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class UserModel : IdentityUser, IModel
    {
        private ICollection<UserRoleModel> _roles;
        public UserModel() {}
        private UserModel(ILazyLoader lazyLoader) { _lazyloader = lazyLoader; }
        private ILazyLoader _lazyloader;

        public string FirstName { get; set; }
        
        public string LastName { get; set; }

        public virtual DepartmentModel Department { get; set; }

        public ICollection<UserRoleModel> Roles { get => _lazyloader.Load(this, ref _roles); set => _roles = value; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}