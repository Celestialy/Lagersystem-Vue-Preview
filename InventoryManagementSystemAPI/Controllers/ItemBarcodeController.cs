using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.DTOs;
using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, InventoryManager")]
    public class ItemBarcodeController : ControllerBase
    {
        private DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public ItemBarcodeController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/itemBarcode
        [HttpGet]
        [Route("find_item")]
        public async Task<IActionResult> FindItem([FromQuery] FindBarcodeItemDTO findBarcodeItemDTO)
        {
            var user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (!_context.Barcodes.Any(x => x.Barcode == findBarcodeItemDTO.Barcode))
                return NotFound("Item not found");

            var itemBarcode = await _context.Barcodes.Where(x => (findBarcodeItemDTO.IsLoan ? x.Item is LoanItemModel : x.Item is ConsumptionItemModel) && x.Barcode == findBarcodeItemDTO.Barcode && x.Item.Image.DepartmentId == user.Department.Id).Select(x => new ItemBarcodeResponseDTO
            {
                ItemId = x.Item.Id,
                Brand = x.Item.Brand,
                Model = x.Item.Model,
                Description = x.Item.Description,
                Category = x.Item.Category.CategoryName,
                Image = new ImageResponseDTO
                {
                    ImageId = x.Item.Image.Id,
                    ImageName = x.Item.Image.ImageName,
                    ImageUri = x.Item.Image.ImageUri
                },
                AmountLeft = x.Item.AmountLeft
            }).FirstOrDefaultAsync();

            if (itemBarcode == null)
                return NotFound("Item not found");

            return Ok(itemBarcode);
        }

        // POST: api/itemBarcode
        [HttpPost]
        [Route("add_barcode_to_item")]
        public async Task<IActionResult> AddBarcodeToItem([FromBody] AddBarcodeItemDTO addBarcodeItemDTO)
        {
            if (!IsDigitsOnly(addBarcodeItemDTO.Barcode))
                return BadRequest("Barcode must only contain digits");

            if (addBarcodeItemDTO.Barcode.Length != 13 && addBarcodeItemDTO.Barcode.Length != 10)
                return BadRequest("Barcode length must be 10 or 13 characters (Digits only)");

            ItemModel item;

            if (addBarcodeItemDTO.IsLoanItem)
            {
                if (!_context.LoanItems.Any(x => x.Id == addBarcodeItemDTO.ItemId))
                    return BadRequest("Item not found");

                if (_context.Barcodes.Any(x => x.Item.Id != addBarcodeItemDTO.ItemId && x.Barcode == addBarcodeItemDTO.Barcode))
                    return BadRequest("An item already has this barcode");

                item = _context.LoanItems.FirstOrDefault(x => x.Id == addBarcodeItemDTO.ItemId);
                item.AmountLeft++;
            }
            else
            {
                if (!_context.ConsumptionItems.Any(x => x.Id == addBarcodeItemDTO.ItemId))
                    return BadRequest("Item not found");

                if (_context.Barcodes.Any(x => x.Item.Id != addBarcodeItemDTO.ItemId && x.Barcode == addBarcodeItemDTO.Barcode))
                    return BadRequest("An item already has this barcode");

                item = _context.ConsumptionItems.FirstOrDefault(x => x.Id == addBarcodeItemDTO.ItemId);
                item.AmountLeft += addBarcodeItemDTO.Amount;
            }

            BarcodeModel barcode = new BarcodeModel()
            {
                Barcode = addBarcodeItemDTO.Barcode,
                Item = item,
                IsAvailable = true
            };

            _context.Barcodes.Add(barcode);
            await _context.SaveChangesAsync();

            return Ok("Successfully created barcode");
        }

        [HttpDelete]
        [Route("remove_barcode_from_item")]
        public async Task<IActionResult> RemoveBarcodeFromItem([FromQuery] RemoveBarcodeFromItemDTO removeBarcodeFromItemDTO)
        {
            if (!_context.Barcodes.Any(x => (removeBarcodeFromItemDTO.IsConsumptionItem ? x.Item is ConsumptionItemModel : x.Item is LoanItemModel) && x.Barcode == removeBarcodeFromItemDTO.Barcode))
                return BadRequest("Item not found");

            BarcodeModel barcode = _context.Barcodes.FirstOrDefault(x => (removeBarcodeFromItemDTO.IsConsumptionItem ? x.Item is ConsumptionItemModel : x.Item is LoanItemModel) && x.Barcode == removeBarcodeFromItemDTO.Barcode);

            if (barcode == null)
                return BadRequest("This item doesn't have a barcode");

            if (removeBarcodeFromItemDTO.IsConsumptionItem)
                barcode.Item.AmountLeft -= removeBarcodeFromItemDTO.Amount;
            else
                barcode.Item.AmountLeft--;
        
            _context.Barcodes.Remove(barcode);
            await _context.SaveChangesAsync();
        
            return Ok("Successfully removed barcode");
        }

        /// <summary>
        /// Checks if a string only contains digits
        /// </summary>
        private bool IsDigitsOnly(string str)
        {
            foreach (char c in str)
                if (c < '0' || c > '9')
                    return false;

            return true;
        }
    }
}