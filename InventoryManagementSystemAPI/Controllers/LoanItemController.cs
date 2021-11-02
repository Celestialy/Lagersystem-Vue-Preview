using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystemAPI.Models;
using InventoryManagementSystemAPI.Database;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystemAPI.DTOs;
using System.IO;
using Microsoft.WindowsAzure.Storage.Blob;
using System.Collections.Generic;
using Microsoft.AspNetCore.Http;
using InventoryManagementSystemAPI.Controllers.Azure;

namespace InventoryManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Manager, InventoryManager")]
    public class LoanItemController : Controller
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public LoanItemController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/loanItem
        [HttpGet]
        [Route("get_all_loan_items")]
        public async Task<IActionResult> GetAllLoanItems([FromQuery] GetLoanItemsDTO getLoanItems)
        {
            var loanItems = await _context.LoanItems.Where(x => x.Inventory.Id == getLoanItems.InventoryId).Select(x => new LoanItemWithCategoryResponseDTO
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
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.Inventory.Id,
                    Name = x.Inventory.Name,
                    Address = x.Inventory.Address,
                    Zipcode = x.Inventory.Zipcode,
                    City = x.Inventory.City,
                    InventoryType = x.Inventory.InventoryType
                },
                AmountLeft = x.Barcodes.Where(z => z.IsAvailable).Count()
            }).ToListAsync();

            if (loanItems.Count == 0)
                return NotFound("Items not found");

            return Ok(loanItems);
        }

        /*// GET: api/loanItem
        [HttpGet]
        [Route("get_loan_item")]
        public async Task<IActionResult> GetLoanItem([FromQuery] GetLoanItemDTO getLoanItem)
        {
            if (!_context.Inventories.Any(x => x.Id == getLoanItem.InventoryId))
                return NotFound("Inventory not found");

            if (!_context.LoanItems.Any(x => x.Barcodes.Any(y => y.Barcode == getLoanItem.ItemBarcode)))
                return NotFound("Item not found");

            LoanItemWithCategoryResponseDTO loanItem = new LoanItemWithCategoryResponseDTO();

            loanItem = await _context.LoanItems.Where(x => x.Barcodes.Any(y => y.Barcode == getLoanItem.ItemBarcode) && x.Inventory.Id == getLoanItem.InventoryId).Select(x => new LoanItemWithCategoryResponseDTO
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
                },
                AmountLeft = x.AmountLeft
            }).FirstOrDefaultAsync();

            if (loanItem == null)
                return NotFound("Item not found");

            return Ok(loanItem);
        }*/

        // PUT: api/loanItem
        [HttpPut]
        [Route("edit_loan_item")]
        public async Task<IActionResult> EditLoanItem([FromBody] EditLoanItemDTO editLoanItem)
        {
            if (!_context.Inventories.Any(x => x.Id == editLoanItem.InventoryId))
                return NotFound("Inventory not found");

            if (!_context.Categories.Any(x => x.Id == editLoanItem.CategoryId))
                return NotFound("Category not found");

            if (!_context.LoanItems.Any(x => x.Id == editLoanItem.ItemId))
                return NotFound("Item not found");

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == editLoanItem.CategoryId);
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == editLoanItem.ImageId);

            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Brand = editLoanItem.Brand;
            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Model = editLoanItem.Model;
            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Description = editLoanItem.Description;
            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Category = category;
            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Image = image;
            _context.LoanItems.FirstOrDefault(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var loanItem = await _context.LoanItems.Include(i => i.Inventory).Include(im => im.Image).Where(x => x.Id == editLoanItem.ItemId && x.Inventory.Id == editLoanItem.InventoryId).Select(x => new LoanItemWithCategoryResponseDTO
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
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.Inventory.Id,
                    Name = x.Inventory.Name,
                    Address = x.Inventory.Address,
                    Zipcode = x.Inventory.Zipcode,
                    City = x.Inventory.City,
                    InventoryType = x.Inventory.InventoryType
                },
                AmountLeft = x.AmountLeft
            }).ToListAsync();

            return Ok(loanItem);
        }

        // POST: api/loanItem
        [HttpPost]
        [Route("add_loan_item")]
        public async Task<IActionResult> AddLoanItem([FromBody] AddLoanItemDTO addLoanItem)
        {
            if (!_context.Inventories.Any(x => x.Id == addLoanItem.InventoryId))
                return NotFound("Inventory not found");

            if (_context.LoanItems.Include(i => i.Inventory).Any(x => x.Inventory.Id == addLoanItem.InventoryId && x.Brand == addLoanItem.Brand && x.Model == addLoanItem.Model))
                return BadRequest("Item already exists");

            var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.Id == addLoanItem.InventoryId);
            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == addLoanItem.CategoryId);
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;
            var image = await _context.Images.FirstOrDefaultAsync(x => x.Id == addLoanItem.ImageId);

            LoanItemModel loanItem = new LoanItemModel()
            {
                Brand = addLoanItem.Brand,
                Model = addLoanItem.Model,
                Description = addLoanItem.Description,
                Category = category,
                Image = image,
                Inventory = inventory,
                CreatedAt = DateTime.Now
            };

            _context.LoanItems.Add(loanItem);
            await _context.SaveChangesAsync();

            return Ok($"Added item: {addLoanItem.Brand} - {addLoanItem.Model}");
        }

        // DELETE: api/loanItem
        [HttpDelete]
        [Route("delete_loan_item")]
        public async Task<IActionResult> DeleteLoanItem([FromQuery] DeleteLoanItemDTO deleteLoanItem)
        {
            if (!_context.Inventories.Any(x => x.Id == deleteLoanItem.InventoryId))
                return NotFound("Inventory not found");

            if (_context.Inventories.FirstOrDefault(x => x.Id == deleteLoanItem.InventoryId).InventoryType != "Loan")
                return BadRequest("Wrong Inventory");

            if (!_context.LoanItems.Any(x => x.Id == deleteLoanItem.ItemId))
                return NotFound("Item not found");

            if (_context.Barcodes.Any(x => x.Item.Id == deleteLoanItem.ItemId))
                return BadRequest("Item has barcode attached to it");

            if (_context.UserLoans.Any(x => x.LoanItem.Item.Id == deleteLoanItem.ItemId && !x.IsHandedIn))
            {
                var user = _context.UserLoans.Include(u => u.User).FirstOrDefault(x => x.LoanItem.Item.Id == deleteLoanItem.ItemId && !x.IsHandedIn).User;
                return BadRequest($"{user.FirstName} {user.LastName} has borrowed the item");
            }

            var loanItem = await _context.LoanItems.FirstOrDefaultAsync(x => x.Id == deleteLoanItem.ItemId);
            _context.Remove(loanItem);
            await _context.SaveChangesAsync();

            return Ok($"Deleted item: {loanItem.Brand} - {loanItem.Model}");
        }
    }
}