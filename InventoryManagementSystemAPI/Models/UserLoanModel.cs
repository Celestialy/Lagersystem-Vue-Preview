using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class UserLoanModel
    {
        public UserLoanModel()
        {

        }
        private UserLoanModel(ILazyLoader lazyLoader) { _lazyloader = lazyLoader; }
        private ILazyLoader _lazyloader;

        [Key]
        public int Id { get; set; }

        [Required]
        public virtual UserModel User { get; set; }

        private BarcodeModel _LoanItem;
        [Required]
        public virtual BarcodeModel LoanItem { get => _lazyloader.Load(this, ref _LoanItem); set => _LoanItem = value; }

        public bool IsHandedIn { get; set; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }
    }
}