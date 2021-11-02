using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class ItemModel : IModel
    {
        public ItemModel()
        { }

        private ItemModel(ILazyLoader lazyLoader) { _lazyloader = lazyLoader; }
        private ILazyLoader _lazyloader;

        [Key]
        public int Id { get; set; }

        [Required]
        public string Brand { get; set; }

        [Required]
        public string Model { get; set; }

        public string Description { get; set; }

        private CategoryModel _Category;
        [Required]
        public virtual CategoryModel Category { get => _lazyloader.Load(this, ref _Category); set => _Category = value; }

        private List<BarcodeModel> _Barcodes;

        [Required]
        public virtual List<BarcodeModel> Barcodes { get => _lazyloader.Load(this, ref _Barcodes); set => _Barcodes = value; }

        private ImageModel _Image;
        [Required]
        public ImageModel Image { get => _lazyloader.Load(this, ref _Image); set => _Image = value; }

        [Required]
        public int AmountLeft { get; set; }

        private InventoryModel _Inventory;
        public virtual InventoryModel Inventory { get => _lazyloader.Load(this, ref _Inventory); set => _Inventory = value; }

        public DateTime CreatedAt { get; set; }

        public DateTime? UpdatedAt { get; set; }


    }
}