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
    [Authorize(Roles = "Admin, Manager, InventoryManager")]
    public class InventoryController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private readonly UserManager<UserModel> _userManager;

        public InventoryController(DatabaseContext context, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, RoleManager<RoleModel> roleManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/inventory
        [HttpGet]
        [Route("get_all_inventories")]
        public async Task<IActionResult> GetAllInventories()
        {
            var inventories = await _context.Inventories.Include(d => d.Department).Select(x => new InventoryWithDepartmentResponseDTO
            {
                InventoryId = x.Id,
                Name = x.Name,
                Address = x.Address,
                Zipcode = x.Zipcode,
                City = x.City,
                InventoryType = x.InventoryType,
                Department = new DepartmentResponseDTO
                {
                    DepartmentId = x.Department.Id,
                    DepartmentName = x.Department.Name
                }
            }).ToListAsync();

            if (inventories.Count < 1)
                return NotFound("No inventories found");

            return Ok(inventories);
        }

        // GET: api/inventory
        [HttpGet]
        [Route("get_inventories")]
        public async Task<IActionResult> GetInventory()
        {
            var user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (!_context.Departments.Any(x => x.Id == user.Department.Id))
                return NotFound("Department not found");

            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == user.Department.Id);

            if (!_context.Inventories.Any(x => x.Department.Id == department.Id))
                return NotFound("No inventories found");

            var inventories = await _context.Inventories.Include(d => d.Department).Where(x => x.Department.Id == department.Id).Select(x => new InventoryResponseDTO
            {
                InventoryId = x.Id,
                Name = x.Name,
                Address = x.Address,
                Zipcode = x.Zipcode,
                City = x.City,
                InventoryType = x.InventoryType
            }).ToListAsync();

            return Ok(inventories);
        }

        // PUT: api/inventory
        [HttpPut]
        [Route("edit_inventory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> PutInventoryModel([FromBody] EditInventoryDTO editInventory)
        {
            if (!_context.Inventories.Any(x => x.Id == editInventory.InventoryId))
                return NotFound("Inventory not found");

            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).Name = editInventory.InventoryName;
            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).Address = editInventory.Address;
            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).Zipcode = editInventory.ZipCode;
            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).City = editInventory.City;
            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).InventoryType = editInventory.InventoryType;
            _context.Inventories.FirstOrDefault(x => x.Id == editInventory.InventoryId).UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var inventory = await _context.Inventories.Where(x => x.Id == editInventory.InventoryId).Select(x => new InventoryResponseDTO
            {
                InventoryId = editInventory.InventoryId,
                Name = editInventory.InventoryName,
                Address = editInventory.Address,
                Zipcode = editInventory.ZipCode,
                City = editInventory.City,
                InventoryType = editInventory.InventoryType
            }).FirstOrDefaultAsync();

            return Ok(inventory);
        }

        // POST: api/inventory
        [HttpPost]
        [Route("add_inventory")]
        [Authorize(Roles = "Admin")]
        public async Task<ActionResult<InventoryModel>> PostInventoryModel([FromBody] AddInventoryDTO addInventory)
        {
            if (_context.Inventories.Any(x => x.Name == addInventory.InventoryName))
                return BadRequest("Inventory already exists");

            var user = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User));

            if (!_context.Departments.Any(x => x.Id == user.Department.Id))
                return NotFound("Department not found");

            var department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == user.Department.Id);

            InventoryModel inventory = new InventoryModel()
            {
                Name = addInventory.InventoryName,
                Address = addInventory.Address,
                Zipcode = addInventory.ZipCode,
                City = addInventory.City,
                Department = department,
                InventoryType = addInventory.InventoryType,
                CreatedAt = DateTime.Now
            };

            _context.Inventories.Add(inventory);
            await _context.SaveChangesAsync();

            return Ok("Successfully created inventory");
        }

        // DELETE: api/inventory
        [HttpDelete]
        [Route("delete_inventory")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteInventoryModel([FromQuery] DeleteInventoryDTO deleteInventory)
        {
            if (!_context.Inventories.Any(x => x.Id == deleteInventory.InventoryId))
                return NotFound("Inventory not found");

            if (_context.LoanItems.Any(x => x.Inventory.Id == deleteInventory.InventoryId))
                return BadRequest("Items already exists. Remove them to complete this action");

            var inventory = await _context.Inventories.FirstOrDefaultAsync(x => x.Id == deleteInventory.InventoryId);
            _context.Remove(inventory);
            await _context.SaveChangesAsync();

            return Ok("Deleted inventory: " + inventory.Name);
        }
    }
}