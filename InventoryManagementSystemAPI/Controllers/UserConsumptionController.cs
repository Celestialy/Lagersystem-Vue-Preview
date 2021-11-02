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
    [Authorize(Roles = "Manager, InventoryManager, User")]
    public class UserConsumptionController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public UserConsumptionController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/userConsumption
        [HttpGet]
        [Route("get_all_userConsumptions")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetAllUserConsumptions()
        {
            if (_context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department == null)
                return BadRequest("User isn't in a department");

            var departmentId = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department.Id;

            var userConsumptions = await _context.UserConsumptions.Where(x => x.User.Department.Id == departmentId).Select(x => new GetAllUserConsumptionHistoryResponseDTO
            {
                User = new BasicUserResponseDTO
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName
                },
                ConsumptionItem =  new BasicItemResponseDTO()
                {
                    ItemId = x.ConsumptionItem.Item.Id,
                    Brand = x.ConsumptionItem.Item.Brand,
                    Model = x.ConsumptionItem.Item.Model,
                    Description = x.ConsumptionItem.Item.Description,
                    Category = x.ConsumptionItem.Item.Category.CategoryName,
                    ImageUri = x.ConsumptionItem.Item.Image.ImageUri.ToString(),
                    Amount = x.Amount
                },
                ConsumptionDate = x.CreatedAt.ToString("dd-MM-yyyy H:mm")
            }).ToListAsync();

            if (userConsumptions.Count == 0)
                return NotFound("No consumptions found");
            userConsumptions.Sort((x, y) => y.ConsumptionDate.CompareTo(x.ConsumptionDate));
            return Ok(userConsumptions);
        }

        // GET: api/userConsumption
        [HttpGet]
        [Route("get_userConsumption")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetUserConsumption([FromQuery] GetUserConsumptionDTO getUserConsumption)
        {
            string userId = getUserConsumption.UserId;

            if (getUserConsumption.UserId == null)
                userId = _userManager.GetUserId(User);
            else if (getUserConsumption.UserId != null && !_context.Users.Any(x => x.Id == getUserConsumption.UserId))
                return NotFound("User not found");

            if (!_context.UserConsumptions.Any(x => x.ConsumptionItem.Item.Id == getUserConsumption.ItemId))
                return NotFound("Item not found");

            var consumptionItems = await _context.UserConsumptions.Where(x => x.User.Id == userId && x.ConsumptionItem.Item.Id == getUserConsumption.ItemId).Select(x => new UserConsumptionItemResponseDTO
            {
                ItemId = x.ConsumptionItem.Item.Id,
                Brand = x.ConsumptionItem.Item.Brand,
                Model = x.ConsumptionItem.Item.Model,
                Description = x.ConsumptionItem.Item.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.ConsumptionItem.Item.Category.Id,
                    CategoryName = x.ConsumptionItem.Item.Category.CategoryName
                },
                Amount = x.Amount,
                Image = new ImageResponseDTO
                {
                    ImageId = x.ConsumptionItem.Item.Image.Id,
                    ImageName = x.ConsumptionItem.Item.Image.ImageName,
                    ImageUri = x.ConsumptionItem.Item.Image.ImageUri
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.ConsumptionItem.Item.Inventory.Id,
                    Name = x.ConsumptionItem.Item.Inventory.Name,
                    Address = x.ConsumptionItem.Item.Inventory.Address,
                    Zipcode = x.ConsumptionItem.Item.Inventory.Zipcode,
                    City = x.ConsumptionItem.Item.Inventory.City,
                    InventoryType = x.ConsumptionItem.Item.Inventory.InventoryType
                },
                Date = x.CreatedAt
            }).FirstOrDefaultAsync();

            var userConsumption = await _context.UserConsumptions.Include(u => u.User).Where(x => x.User.Id == userId).Select(x => new UserConsumptionResponseDTO
            {
                User = new UserResponseDTO
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Roles = x.User.Roles.Select(x => x.Role.Name).ToList(),
                },
                UserConsumptionItem = consumptionItems
            }).FirstOrDefaultAsync();

            return Ok(userConsumption);
        }

        // GET: api/userConsumption
        [HttpGet]
        [Route("get_userConsumption_history")]
        public async Task<IActionResult> GetUserConsumptionHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!_context.UserConsumptions.Any(x => x.User.Id == user.Id))
                return NotFound("No items found");

            var consumptionItems = await _context.UserConsumptions.Where(x => x.User.Id == user.Id).Select(x => new UserConsumptionItemResponseDTO
            {
                ItemId = x.ConsumptionItem.Item.Id,
                Brand = x.ConsumptionItem.Item.Brand,
                Model = x.ConsumptionItem.Item.Model,
                Description = x.ConsumptionItem.Item.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.ConsumptionItem.Item.Category.Id,
                    CategoryName = x.ConsumptionItem.Item.Category.CategoryName
                },
                Amount = x.Amount,
                Image = new ImageResponseDTO
                {
                    ImageId = x.ConsumptionItem.Item.Image.Id,
                    ImageName = x.ConsumptionItem.Item.Image.ImageName,
                    ImageUri = x.ConsumptionItem.Item.Image.ImageUri
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.ConsumptionItem.Item.Inventory.Id,
                    Name = x.ConsumptionItem.Item.Inventory.Name,
                    Address = x.ConsumptionItem.Item.Inventory.Address,
                    Zipcode = x.ConsumptionItem.Item.Inventory.Zipcode,
                    City = x.ConsumptionItem.Item.Inventory.City,
                    InventoryType = x.ConsumptionItem.Item.Inventory.InventoryType
                },
                Date = x.CreatedAt
            }).ToListAsync();

            var userConsumption = await _context.UserConsumptions.Include(u => u.User).Where(x => x.User.Id == user.Id).Select(x => new UserConsumptionHistoryResponseDTO
            {
                User = new UserResponseDTO
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                    Email = x.User.Email,
                    Roles = x.User.Roles.Select(x => x.Role.Name).ToList(),
                },
                ConsumptionItems = consumptionItems
            }).FirstOrDefaultAsync();

            return Ok(userConsumption);
        }

        // POST: api/userConsumption
        [HttpPost]
        [Route("add_userConsumption")]
        public async Task<IActionResult> AddUserConsumption([FromBody] AddUserConsumptionDTO addUserConsumptionDTO)
        {
            UserModel user = await _userManager.GetUserAsync(User);

            if (addUserConsumptionDTO.UserId != null)
                user = await _userManager.FindByIdAsync(addUserConsumptionDTO.UserId);

            //checking for items with the same barcode
            if (!_context.ConsumptionItems.Any(x => x.Barcodes.Any(y => addUserConsumptionDTO.ItemBarcode == y.Barcode)))
                return NotFound("Item not found");

            var item = await _context.Barcodes.FirstOrDefaultAsync(x => x.Item is ConsumptionItemModel && addUserConsumptionDTO.ItemBarcode == x.Barcode);
            UserConsumptionsModel userConsumption = new UserConsumptionsModel();

            if (item.Item.AmountLeft >= addUserConsumptionDTO.ItemAmount)
            {
                userConsumption = new UserConsumptionsModel
                {
                    User = user,
                    ConsumptionItem = item,
                    Amount = addUserConsumptionDTO.ItemAmount,
                    CreatedAt = DateTime.Now
                };

                _context.UserConsumptions.Add(userConsumption);
                item.Item.AmountLeft -= addUserConsumptionDTO.ItemAmount;
            }
            else
                return BadRequest("Not enough items in the inventory");

            await _context.SaveChangesAsync();

            if (addUserConsumptionDTO.ItemAmount > 1)
                return Ok($"{user.UserName} took some items");
            else
                return Ok($"{user.UserName} took an item");
        }
    }
}