using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class RoleUserResponseDTO
    {
    [Required]
    public string RoleName { get; set; }

    public List<UserResponseDTO> Users { get; set; }
    }   
}
