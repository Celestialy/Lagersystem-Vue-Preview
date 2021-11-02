using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class InventoryModel : IModel
    {
        [Key]
        public int Id { get; set; }

        [Required]
        public string Name { get; set; }

        [Required]
        public string Address { get; set; }

        [Required]
        public string Zipcode { get; set; }

        [Required]
        public string City { get; set; }

        [Required]
        public string InventoryType { get; set; }

        [Required]
        public virtual DepartmentModel Department { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}