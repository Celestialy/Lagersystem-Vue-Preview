using InventoryManagementSystemAPI.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class AddDepartmentDTO
    {
        public string DepartmentName { get; set; }
    }

    public class GetDepartmentDTO
    {
        [Required]
        public int DepartmentId { get; set; }
    }

    public class EditDepartmentDTO
    {
        [Required]
        public int DepartmentId { get; set; }

        [Required]
        public string NewDepartmentName { get; set; }
    }
}