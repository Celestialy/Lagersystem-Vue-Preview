using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystemAPI.Models;
using InventoryManagementSystemAPI.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystemAPI.DTOs;

namespace InventoryManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, InventoryManager")]
    public class ConsumptionItemController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public ConsumptionItemController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/consumptionItem
        [HttpGet]
        [Route("get_all_consumption_items")]
        public async Task<IActionResult> GetConsumptions([FromQuery] GetConsumptionItemsDTO getConsumptionItems)
        {
            if (!_context.Inventories.Any(x => x.Id == getConsumptionItems.InventoryId))
                return NotFound("Inventory not found");

            if (!_context.ConsumptionItems.Any(x => x.Inventory.Id == getConsumptionItems.InventoryId))
                return NotFound("Items not found");

            var consumptionItems = await _context.ConsumptionItems.Include(i => i.Inventory).Include(im => im.Image).Where(x => x.Inventory.Id == getConsumptionItems.InventoryId).Select(x => new ConsumptionItemWithCategoryResponseDTO
            {
                ItemId = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Description = x.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.Category.Id,
                    CategoryName = x.Category.CategoryName
                },
                Barcodes = x.Barcodes.Select(x => x.Barcode).ToList(),
                AmountLeft = x.AmountLeft,
                Image = new ImageResponseDTO
                {
                    ImageId = x.Image.Id,
                    ImageName = x.Image.ImageName,
                    ImageUri = x.Image.ImageUri
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.Inventory.Id,
                    Name = x.Inventory.Name,
                    Address = x.Inventory.Address,
                    Zipcode = x.Inventory.Zipcode,
                    City = x.Inventory.City,
                    InventoryType = x.Inventory.InventoryType
                }
            }).ToListAsync();

            return Ok(consumptionItems);
        }

       /* // GET: api/consumption
        [HttpGet]
        [Route("get_consumption_item")]
        public async Task<IActionResult> GetConsumptionItem([FromQuery] GetConsumptionItemDTO getConsumptionItem)
        {
            if (!_context.Inventories.Any(x => x.Id == getConsumptionItem.InventoryId))
                return NotFound("Inventory not found");

            if (_context.Inventories.FirstOrDefault(x => x.Id == getConsumptionItem.InventoryId).InventoryType == "Loan")
                return BadRequest("Wrong Inventory");

            if (!_context.ConsumptionItems.Any(x => x.Id == getConsumptionItem.ItemId))
                return NotFound("Item not found");

            var consumptionItems = await _context.ConsumptionItems.Where(x => x.Id == getConsumptionItem.ItemId && x.Inventory.Id == getConsumptionItem.InventoryId).Select(x => new ConsumptionItemWithCategoryResponseDTO
            {
                ItemId = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Description = x.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.Category.Id,
                    CategoryName = x.Category.CategoryName
                },
                Barcodes = x.Barcodes.Select(x => x.Barcode).ToList(),
                AmountLeft = x.AmountLeft,
                Image = new ImageResponseDTO
                {
                    ImageId = x.Image.Id,
                    ImageName = x.Image.ImageName,
                    ImageUri = x.Image.ImageUri,
                    DepartmentId = x.Image.DepartmentId
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.Inventory.Id,
                    Name = x.Inventory.Name,
                    Address = x.Inventory.Address,
                    Zipcode = x.Inventory.Zipcode,
                    City = x.Inventory.City,
                    InventoryType = x.Inventory.InventoryType
                }
            }).ToListAsync();

            return Ok(consumptionItems);
        }*/

        // PUT: api/consumption
        [HttpPut]
        [Route("edit_consumption_item")]
        public async Task<IActionResult> EditConsumptionItem([FromBody] EditConsumptionItemDTO editConsumptionItem)
        {
            if (!_context.Inventories.Any(x => x.Id == editConsumptionItem.InventoryId))
                return NotFound("Inventory not found");

            if (_context.Inventories.FirstOrDefault(x => x.Id == editConsumptionItem.InventoryId).InventoryType == "Loan")
                return BadRequest("Wrong Inventory");

            if (!_context.Categories.Any(x => x.Id == editConsumptionItem.CategoryId))
                return NotFound("Category not found");

            if (!_context.ConsumptionItems.Any(x => x.Id == editConsumptionItem.ItemId))
                return NotFound("Item not found");

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == editConsumptionItem.CategoryId);
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == editConsumptionItem.ImageId);

            var item = _context.ConsumptionItems.FirstOrDefault(x => x.Id == editConsumptionItem.ItemId && x.Inventory.Id == editConsumptionItem.InventoryId);
            item.Brand = editConsumptionItem.Brand;
            item.Model = editConsumptionItem.Model;
            item.Description = editConsumptionItem.Description;
            item.Category = category;
            item.Image = image;
            item.AmountLeft = editConsumptionItem.AmountLeft;
            item.UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();
            
            var consumptionItem = await _context.ConsumptionItems.Where(x => x.Id == editConsumptionItem.ItemId && x.Inventory.Id == editConsumptionItem.InventoryId).Select(x => new ConsumptionItemWithCategoryResponseDTO
            {
                ItemId = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Description = x.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.Category.Id,
                    CategoryName = x.Category.CategoryName
                },
                Barcodes = x.Barcodes.Select(x => x.Barcode).ToList(),
                Image = new ImageResponseDTO
                {
                    ImageId = x.Image.Id,
                    ImageName = x.Image.ImageName,
                    ImageUri = x.Image.ImageUri
                },
                AmountLeft = x.AmountLeft,
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.Inventory.Id,
                    Name = x.Inventory.Name,
                    Address = x.Inventory.Address,
                    Zipcode = x.Inventory.Zipcode,
                    City = x.Inventory.City,
                    InventoryType = x.Inventory.InventoryType
                }
            }).ToListAsync();

            return Ok(consumptionItem);
        }

        // POST: api/consumption
        [HttpPost]
        [Route("add_consumption_item")]
        public async Task<IActionResult> AddConsumptionItem([FromBody] AddConsumptionItemDTO addConsumptionItem )
        {
            if (!_context.Inventories.Any(x => x.Id == addConsumptionItem.InventoryId))
                return NotFound("Inventory not found");

            if (!_context.Categories.Any(x => x.Id == addConsumptionItem.CategoryId))
                return NotFound("Category not found");

            if (!_context.Images.Any(x => x.Id == addConsumptionItem.ImageId))
                return NotFound("Image not found");

            if (_context.ConsumptionItems.Any(x => x.Brand == addConsumptionItem.Brand && x.Model == addConsumptionItem.Model))
                return BadRequest("Item already exists");


            var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.Id == addConsumptionItem.InventoryId);
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == addConsumptionItem.CategoryId);
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == addConsumptionItem.ImageId);

            var consumptionItem = new ConsumptionItemModel()
            {
                Brand = addConsumptionItem.Brand,
                Model = addConsumptionItem.Model,
                Description = addConsumptionItem.Description,
                Category = category,
                Image = image,
                Inventory = inventory,
                CreatedAt = DateTime.Now
            };

            _context.ConsumptionItems.Add(consumptionItem);
            await _context.SaveChangesAsync();

            return Ok($"Added item: {addConsumptionItem.Brand} - {addConsumptionItem.Model}");
        }

        // DELETE: api/consumption
        [HttpDelete]
        [Route("delete_consumption_item")]
        public async Task<IActionResult> DeleteConsumptionItem([FromQuery] GetConsumptionItemDTO getConsumptionItem)
        {
            if (!_context.Inventories.Any(x => x.Id == getConsumptionItem.InventoryId))
                return NotFound("Inventory not found");

            if (_context.Inventories.FirstOrDefault(x => x.Id == getConsumptionItem.InventoryId).InventoryType == "Loan")
                return BadRequest("Wrong Inventory");

            if (!_context.ConsumptionItems.Any(x => x.Id == getConsumptionItem.ItemId))
                return NotFound("Item not found");

            if (_context.Barcodes.Any(x => x.Item.Id == getConsumptionItem.ItemId))
                return BadRequest("Item has barcode attached to it");

            var consumptionItem = await _context.ConsumptionItems.FirstOrDefaultAsync(x => x.Id == getConsumptionItem.ItemId);
            _context.Remove(consumptionItem);
            await _context.SaveChangesAsync();

            return Ok($"Deleted item: {consumptionItem.Brand} - {consumptionItem.Model}");
        }
    }
}