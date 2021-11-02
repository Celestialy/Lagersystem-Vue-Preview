using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class UserResponseDTO
    {
        [Required]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }
    }

    public class BasicUserResponseDTO
    {
        [Required]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }
    }

    public class UserWithDepartmentResponseDTO
    {
        [Required]
        public string UserId { get; set; }

        public string Username { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string Email { get; set; }

        public List<string> Roles { get; set; }

        public DepartmentResponseDTO Department { get; set; }
    }
}
