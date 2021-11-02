using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
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
    public class CategoryController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public CategoryController(DatabaseContext context, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, RoleManager<RoleModel> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/category
        [HttpGet]
        [Route("get_all_categories")]
        public async Task<IActionResult> GetCategories([FromQuery]CategoryModeDTO categoryMode)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            switch (categoryMode)
            {
                case CategoryModeDTO.standard:
                    {
                        var categories = await _context.Categories.Include(d => d.Department).ThenInclude(i => i.Inventories).Select(x => new CategoryResponseDTO
                        {
                            CategoryID = x.Id,
                            CategoryName = x.CategoryName,
                        }).ToListAsync();

                        if (categories.Count() >= 1)
                            return Ok(categories);
                        else
                            return NotFound("No categories found");
                    }
                case CategoryModeDTO.WitchCheck:
                    {
                        var categories = await _context.Categories.Include(d => d.Department).ThenInclude(i => i.Inventories).Select(x => new CategoryWithIsUsedResponseDTO
                        {
                            CategoryID = x.Id,
                            CategoryName = x.CategoryName,
                            IsUsed = _context.Items.Any(z => z.Category == x)
                        }).ToListAsync();

                        if (categories.Count() >= 1)
                            return Ok(categories);
                        else
                            return NotFound("No categories found");
                    }
                default:
                    return BadRequest("Invalid Mode");
            }
            
        }

        // GET: api/category
        [HttpGet]
        [Route("get_category_with_loan_items")]
        public async Task<IActionResult> GetCategoriesWithLoanItems([FromQuery] GetCategoryDTO getCategoryDTO)
        {
            if (!_context.Categories.Any(x => x.Id == getCategoryDTO.CategoryId))
                return NotFound("Category not found");

            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            var items = await _context.LoanItems.Include(c => c.Category).Where(x => x.Category.Id == getCategoryDTO.CategoryId).Select(x => new LoanItemResponseDTO
            {
                ItemId = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Description = x.Description,
                Barcodes = x.Barcodes.Select(z => z.Barcode).ToList(),
                Image = new ImageResponseDTO { 
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

            var categories = await _context.Categories.Include(d => d.Department).Where(x => x.Id == getCategoryDTO.CategoryId && x.Department.Id == department.Id).Select(x => new CategoryWithLoanItemsResponseDTO
            {   
                CategoryID = x.Id,
                CategoryName = x.CategoryName,
                LoanItems = items
            }).FirstOrDefaultAsync();

            return Ok(categories);
        }

        // GET: api/category
        [HttpGet]
        [Route("get_category_with_consumption_items")]
        public async Task<IActionResult> GetCategoriesWithConsumptionItems([FromQuery] GetCategoryDTO getCategoryDTO)
        {
            if (!_context.Categories.Any(x => x.Id == getCategoryDTO.CategoryId))
                return NotFound("Category not found");

            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;

            var items = await _context.ConsumptionItems.Include(c => c.Category).Where(x => x.Category.Id == getCategoryDTO.CategoryId).Select(x => new ConsumptionItemResponseDTO
            {
                ItemId = x.Id,
                Brand = x.Brand,
                Model = x.Model,
                Description = x.Description,
                Barcodes = x.Barcodes.Select(z => z.Barcode).ToList(),
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

            var categories = await _context.Categories.Include(d => d.Department).Where(x => x.Id == getCategoryDTO.CategoryId && x.Department.Id == department.Id).Select(x => new CategoryWithConsumptionItemsResponseDTO
            {
                CategoryID = x.Id,
                CategoryName = x.CategoryName,
                ConsumptionItemResponses = items
            }).FirstOrDefaultAsync();

            return Ok(categories);
        }

        // PUT: api/category
        [HttpPut]
        [Route("edit_category")]
        public async Task<IActionResult> EditCategory([FromBody] EditCategoryDTO editCategoryDTO)
        {
            var departmentId = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department.Id;
            if (!_context.Categories.Any(x => x.Id == editCategoryDTO.CategoryId && x.Department.Id == departmentId))
                return NotFound("Category not found");

            var Category = await _context.Categories.FirstOrDefaultAsync(x => x.Id == editCategoryDTO.CategoryId && x.Department.Id == departmentId);
            Category.CategoryName = editCategoryDTO.CategoryName;
            Category.UpdatedAt = DateTime.Now;
            
            await _context.SaveChangesAsync();

            var category = await _context.Categories.Where(x => x.Id == editCategoryDTO.CategoryId).Select(x => new CategoryResponseDTO
            {
                CategoryID = x.Id,
                CategoryName = x.CategoryName
            }).FirstOrDefaultAsync();

            return Ok(category);
        }

        // POST: api/category
        [HttpPost]
        [Route("add_category")]
        public async Task<IActionResult> AddCategory([FromBody] AddCategoryDTO addCategoryDTO)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;
            if (_context.Categories.Any(x => x.Department.Id == department.Id && x.CategoryName == addCategoryDTO.CategoryName))
                return BadRequest("Category already exists");

            CategoryModel category = new CategoryModel()
            {
                CategoryName = addCategoryDTO.CategoryName,
                Department = department,
                CreatedAt = DateTime.Now
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return Ok("Added item: " + addCategoryDTO.CategoryName);
        }

        // POST: api/category
        [HttpDelete]
        [Route("delete_category")]
        public async Task<IActionResult> DeleteCategory([FromQuery] DeleteCategoryDTO deleteCategory)
        {
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;
            if (!_context.Categories.Any(x => x.Department.Id == department.Id && x.Id == deleteCategory.CategoryId))
                return BadRequest("Category doesnt exists");

            if (_context.ConsumptionItems.Any(x=> x.Category.Id == deleteCategory.CategoryId) || (_context.LoanItems.Any(x => x.Category.Id == deleteCategory.CategoryId)))
                return BadRequest("Category is in use");

            var category = await _context.Categories.FirstOrDefaultAsync(x => x.Department.Id == department.Id && x.Id == deleteCategory.CategoryId);
            var name = category.CategoryName;
            _context.Remove(category);
            await _context.SaveChangesAsync();

            return Ok("Removed item: " + name);
        }
    }
}