using Microsoft.EntityFrameworkCore.Infrastructure;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Threading.Tasks;

namespace InventoryManagementSystemAPI.Models
{
    public class BarcodeModel
    {
        public BarcodeModel() { }

        private BarcodeModel(ILazyLoader lazyLoader) { _lazyloader = lazyLoader; }
        private ILazyLoader _lazyloader;

        [Key]
        public int BarcodeId { get; set; }

        [Required]
        public string Barcode { get; set; }

        private ItemModel _Item;

        [Required]
        public ItemModel Item { get => _lazyloader.Load(this, ref _Item); set => _Item = value; }

        public bool IsAvailable { get; set; }
    }
}