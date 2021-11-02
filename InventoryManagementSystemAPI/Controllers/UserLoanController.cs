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
    public class UserLoanController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public UserLoanController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            this._context = context;
            this._userManager = userManager;
        }

        // GET: api/userLoan
        [HttpGet]
        [Route("get_user_loans")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetUserLoans()
        {
            await Task.Delay(1);
            return Ok();
        }

        // GET: api/userLoan
        [HttpGet]
        [Route("get_all_loans")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetAllUserLoans()
        {
            if (_context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department == null)
                return BadRequest("User isn't in a department");

            var departmentId = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department.Id;

            var userLoans = await _context.UserLoans.Where(x => x.User.Department.Id == departmentId).Select(x => new GetAllUserLoansResponseDTO
            {
                User = new BasicUserResponseDTO
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                },
                LoanItem = new BasicItemResponseDTO()
                {
                    ItemId = x.LoanItem.Item.Id,
                    Brand = x.LoanItem.Item.Brand,
                    Model = x.LoanItem.Item.Model,
                    Description = x.LoanItem.Item.Description,
                    Category = x.LoanItem.Item.Category.CategoryName,
                    ImageUri = x.LoanItem.Item.Image.ImageUri.ToString(),
                },
                IsReturned = x.IsHandedIn,
                LoanDate = x.CreatedAt.ToString("dd-MM-yyyy H:mm"),
                ReturnDate = x.UpdatedAt.GetValueOrDefault().ToString("dd-MM-yyyy H:mm")
            }).ToListAsync();

            if (userLoans.Count == 0)
                return NotFound("No loans found");

            userLoans.Sort((x, y) => y.LoanDate.CompareTo(x.LoanDate));
            return Ok(userLoans);
        }

        // GET: api/userLoan
        [HttpGet]
        [Route("get_userLoan")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetUserLoan([FromQuery] GetUserLoanDTO getUserLoanDTO)
        {
            var loanItem = await _context.UserLoans.Where(x => x.LoanItem.Barcode == getUserLoanDTO.ItemBarcode).Select(x => new UserLoanItemResponseDTO
            {
                ItemId = x.LoanItem.Item.Id,
                Brand = x.LoanItem.Item.Brand,
                Model = x.LoanItem.Item.Model,
                Description = x.LoanItem.Item.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.LoanItem.Item.Category.Id,
                    CategoryName = x.LoanItem.Item.Category.CategoryName
                },
                Image = new ImageResponseDTO
                {
                    ImageId = x.LoanItem.Item.Image.Id,
                    ImageName = x.LoanItem.Item.Image.ImageName,
                    ImageUri = x.LoanItem.Item.Image.ImageUri
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.LoanItem.Item.Inventory.Id,
                    Name = x.LoanItem.Item.Inventory.Name,
                    Address = x.LoanItem.Item.Inventory.Address,
                    Zipcode = x.LoanItem.Item.Inventory.Zipcode,
                    City = x.LoanItem.Item.Inventory.City,
                    InventoryType = x.LoanItem.Item.Inventory.InventoryType
                },
                IsAvailable = x.LoanItem.IsAvailable,
                CreatedAt = x.CreatedAt.ToString("dd-MM-yyyy H:mm"),
                UpdatedAt = x.UpdatedAt.GetValueOrDefault().ToString("dd-MM-yyyy H:mm")
            }).FirstOrDefaultAsync();

            var users = await _context.UserLoans.Where(x => x.LoanItem.Barcode == getUserLoanDTO.ItemBarcode && !x.IsHandedIn).Select(x => new BasicUserResponseDTO
            {
                UserId = x.User.Id,
                Username = x.User.UserName,
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
            }).ToListAsync();

            var loan = await _context.UserLoans.Select(x => new UserLoanResponseDTO
            {
                Users = users,
                LoanItem = loanItem
            }).FirstOrDefaultAsync();

            if (loan == null)
                return BadRequest("Items not found");

            return Ok(loan);
        }

        // GET: api/userLoan
        [HttpGet]
        [Route("get_userLoan_history")]
        public async Task<IActionResult> GetUserLoanHistory()
        {
            var user = await _userManager.GetUserAsync(User);

            if (!_context.UserLoans.Any(x => x.User.Id == user.Id))
                return NotFound("No items found");

            var loanItems = await _context.UserLoans.Where(x => x.User.Id == user.Id).Select(x => new UserLoanItemResponseDTO
            {
                ItemId = x.LoanItem.Item.Id,
                Brand = x.LoanItem.Item.Brand,
                Model = x.LoanItem.Item.Model,
                Description = x.LoanItem.Item.Description,
                Category = new CategoryResponseDTO
                {
                    CategoryID = x.LoanItem.Item.Category.Id,
                    CategoryName = x.LoanItem.Item.Category.CategoryName
                },
                Image = new ImageResponseDTO
                {
                    ImageId = x.LoanItem.Item.Image.Id,
                    ImageName = x.LoanItem.Item.Image.ImageName,
                    ImageUri = x.LoanItem.Item.Image.ImageUri,
                },
                Inventory = new InventoryResponseDTO
                {
                    InventoryId = x.LoanItem.Item.Inventory.Id,
                    Name = x.LoanItem.Item.Inventory.Name,
                    Address = x.LoanItem.Item.Inventory.Address,
                    Zipcode = x.LoanItem.Item.Inventory.Zipcode,
                    City = x.LoanItem.Item.Inventory.City,
                    InventoryType = x.LoanItem.Item.Inventory.InventoryType
                },
                IsAvailable = x.IsHandedIn,
                CreatedAt = x.CreatedAt.ToString("dd-MM-yyyy H:mm"),
                UpdatedAt = x.UpdatedAt.GetValueOrDefault().ToString("dd-MM-yyyy H:mm")
            }).ToListAsync();

            var userLoans = await _context.UserLoans.Include(u => u.User).Where(x => x.User.Id == user.Id).Select(x => new UserLoanHistoryResponseDTO
            {
                User = new BasicUserResponseDTO
                {
                    UserId = x.User.Id,
                    Username = x.User.UserName,
                    FirstName = x.User.FirstName,
                    LastName = x.User.LastName,
                },
                LoanItems = loanItems
            }).FirstOrDefaultAsync();

            return Ok(userLoans);
        }

        // GET: api/userLoan
        [HttpGet]
        [Route("Find_Loans_By_Barcode")]
        public async Task<IActionResult> FindLoansByBarcode([FromQuery] FindUserLoansByBarcode findUserLoansByBarcode)
        {
            if (!_context.UserLoans.Any(x => x.LoanItem.Barcode == findUserLoansByBarcode.Barcode && !x.IsHandedIn))
                return BadRequest("no user has currently loaning this item");

            var users = await _context.UserLoans.Where(x => x.LoanItem.Barcode == findUserLoansByBarcode.Barcode && !x.IsHandedIn).Select(x => new BasicUserResponseDTO
            {
                FirstName = x.User.FirstName,
                LastName = x.User.LastName,
                Username = x.User.UserName,
                UserId = x.User.Id
            }).ToListAsync();

            return Ok(users);
        }

        // POST: api/userLoan
        [HttpPost]
        [Route("add_loan")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> AddLoanItem([FromBody] AddUserLoanDTO addUserLoanDTO)
        {
            UserModel user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (addUserLoanDTO.UserId != null)
                user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == addUserLoanDTO.UserId);

            //checking for loan items with the same barcode
            if (!_context.LoanItems.Any(x => x.Barcodes.Any(y => addUserLoanDTO.ItemBarcode == y.Barcode) && x.Inventory.Department == user.Department))
                return NotFound("Item not found");

            if (!_context.LoanItems.Any(x => x.Barcodes.Any(y => addUserLoanDTO.ItemBarcode == y.Barcode && y.IsAvailable) && x.Inventory.Department == user.Department))
            {
                UserLoanModel failedItems = _context.UserLoans.Include(u => u.User).FirstOrDefault(x => x.LoanItem.Barcode == addUserLoanDTO.ItemBarcode && !x.IsHandedIn && x.LoanItem.Item.Inventory.Department == user.Department);
                return BadRequest(failedItems.User.UserName + " has allready borrowed: " + failedItems.LoanItem.Item.Brand + " " + failedItems.LoanItem.Item.Model);
            }

            var item = await _context.Barcodes.FirstOrDefaultAsync(x => x.Item is LoanItemModel && addUserLoanDTO.ItemBarcode == x.Barcode && x.IsAvailable && x.Item.Inventory.Department == user.Department);
            UserLoanModel userLoan = new UserLoanModel();

            userLoan = new UserLoanModel()
            {
                User = user,
                LoanItem = item,
                IsHandedIn = false,
                CreatedAt = DateTime.Now
            };

            _context.UserLoans.Add(userLoan);
            item.IsAvailable = false;
            item.Item.AmountLeft--;

            await _context.SaveChangesAsync();
            return Ok($"{user.UserName} borrowed an item");
        }

        // DELETE: api/userLoan
        [HttpDelete]
        [Route("delete_loan")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> DeleteLoanItem([FromQuery] DeleteUserLoanDTO deleteUserLoanDTO)
        {
            UserModel user = await _userManager.GetUserAsync(User);

            if (deleteUserLoanDTO.UserId != null)
                user = await _context.Users.FirstOrDefaultAsync(x => x.Id == deleteUserLoanDTO.UserId);

            if (!_context.LoanItems.Any(x => x.Barcodes.Any(y => deleteUserLoanDTO.ItemBarcode == y.Barcode && !y.IsAvailable)))
                return NotFound("Item not found");

            //var item = await _context.Barcodes.FirstOrDefaultAsync(x => x.Item is LoanItemModel && deleteUserLoanDTO.ItemBarcode == x.Barcode && !x.IsAvailable);

            if (!_context.UserLoans.Any(x => x.User.Id == user.Id && x.LoanItem.Barcode == deleteUserLoanDTO.ItemBarcode && !x.IsHandedIn))
                return BadRequest("Item isn't borrowed by this user");
            var loan = await _context.UserLoans.FirstOrDefaultAsync(x => !x.IsHandedIn && x.User.Id == deleteUserLoanDTO.UserId && x.LoanItem.Barcode == deleteUserLoanDTO.ItemBarcode);
            loan.IsHandedIn = true;
            loan.UpdatedAt = DateTime.Now;
            loan.LoanItem.IsAvailable = true;
            loan.LoanItem.Item.AmountLeft++;

            await _context.SaveChangesAsync();
            return Ok($"{user.UserName} handed in the item");
        }
    }
}