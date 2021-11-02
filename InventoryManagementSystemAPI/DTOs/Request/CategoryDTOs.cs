using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.DTOs
{
    public class GetCategoryDTO
    {
        [Required]
        public int CategoryId { get; set; }
    }

    public class AddCategoryDTO
    {
        [Required]
        public string CategoryName { get; set; }

    }

    public class EditCategoryDTO
    {
        [Required]
        public int CategoryId { get; set; }

        [Required]
        public string CategoryName { get; set; }
    }

    public class DeleteCategoryDTO
    {
        [Required]
        public int CategoryId { get; set; }
    }

    public enum CategoryModeDTO
    {
        standard = 0,
        WitchCheck = 1
    }
}