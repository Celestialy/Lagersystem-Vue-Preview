using System;
using System.ComponentModel.DataAnnotations;

namespace InventoryManagementSystemAPI.DTOs
{

    public class ShowAllUsersDTO
    {
        public int DepartmentId { get; set; }
    }

    public class AddDepartmentUserDTO
    {
        [Required]
        public string UserId { get; set; }

        public int DepartmentId { get; set; } = 0;
    }

    public class RemoveDepermentUserDTO
    {
        [Required]
        public string UserId { get; set; }
    }
}