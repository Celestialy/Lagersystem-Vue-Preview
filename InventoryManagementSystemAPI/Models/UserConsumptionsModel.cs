using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class UserConsumptionsModel
    {
        public UserConsumptionsModel()
        {

        }
        private UserConsumptionsModel(ILazyLoader lazyLoader) { _lazyloader = lazyLoader; }
        private ILazyLoader _lazyloader;

        [Key]
        public int Id { get; set; }

        [Required]
        public virtual UserModel User { get; set; }

        private BarcodeModel _ConsumptionItem;
        [Required]
        public virtual BarcodeModel ConsumptionItem { get => _lazyloader.Load(this, ref _ConsumptionItem); set => _ConsumptionItem = value; }

        [Required]
        public int Amount { get; set; }

        public DateTime CreatedAt { get; set; }
    }
}