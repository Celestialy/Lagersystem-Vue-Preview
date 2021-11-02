using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystemAPI.Models;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.DTOs;
using System.Security.Cryptography;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace InventoryManagementSystemAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin, Manager, InventoryManager")]
    public class UserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;
        private RoleManager<RoleModel> _roleManager;
        private SignInManager<UserModel> _signInManager;

        public UserController(DatabaseContext context, UserManager<UserModel> userManager, RoleManager<RoleModel> roleManager, SignInManager<UserModel> signInManager)
        {
            this._context = context;
            this._userManager = userManager;
            this._roleManager = roleManager;
            this._signInManager = signInManager;
        }

        // GET: api/user
        [HttpGet]
        [Route("get_users_from_department")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetUsersInDepartment()
        {
            var user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (user.Department == null)
                return BadRequest($"{user.UserName} needs a department");

            //var departmentId = _context.Departments.Include(u => u.Users).FirstOrDefault(x => x.Users.Contains(user)).Id;

            var users = _context.Users.Where(x => x.Department.Id == user.Department.Id).Select(x => new UserResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Roles = x.Roles.Select(x => x.Role.Name).ToList()
                //Roles = _context.Roles.Where(z => _userManager.GetRolesAsync(x).Result.Any(y => z.Name == y)).Select(z => new RoleDTO { RoleId = z.Id, Rolename = z.Name}).ToList()
            }).ToList();

            return Ok(users);
        }

        // GET: api/user
        [HttpGet]
        [Route("get_basic_users_from_department")]
        [Authorize(Roles = "Manager, InventoryManager")]
        public async Task<IActionResult> GetBasicUsersInDepartment()
        {
            var user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (user.Department == null)
                return BadRequest($"{user.UserName} needs a department");

            //var departmentId = _context.Departments.Include(u => u.Users).FirstOrDefault(x => x.Users.Contains(user)).Id;

            var users = _context.Users.Where(x => x.Department.Id == user.Department.Id).Select(x => new BasicUserResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                //Roles = _context.Roles.Where(z => _userManager.GetRolesAsync(x).Result.Any(y => z.Name == y)).Select(z => new RoleDTO { RoleId = z.Id, Rolename = z.Name}).ToList()
            }).ToList();

            return Ok(users);
        }

        // GET: api/user
        [HttpGet]
        [Route("get_users_without_department")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetUsersWithoutDepartment()
        {
            var users = _context.Users.Where(x => x.Department == null).Select(x => new UserResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();

            await Task.Delay(1);

            if (users.Count < 1)
                return NotFound("No users found");

            return Ok(users);
        }

        // GET: api/user
        [HttpGet]
        [Route("get_user")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> GetUserModel([FromQuery] GetUser GetUserDTO)
        {
            if (!_context.Users.Any(x => x.Id == GetUserDTO.UserId))
                return NotFound("No user found");

            var departmentId = _context.Users.Include(d => d.Department).FirstOrDefault(u => u.Id == GetUserDTO.UserId).Department.Id;

            var inventories = await _context.Inventories.Where(x => x.Department.Id == departmentId).Select(x => new InventoryResponseDTO
            {
                InventoryId = x.Id,
                Name = x.Name,
                Address = x.Address,
                Zipcode = x.Zipcode,
                City = x.City,
                InventoryType = x.InventoryType
            }).ToListAsync();

            var user = await _context.Users.Include(d => d.Department).Where(x => x.Id == GetUserDTO.UserId).Select(x => new UserWithDepartmentResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email,
                Roles = x.Roles.Select(x => x.Role.Name).ToList(),
                Department = new DepartmentResponseDTO
                {
                    DepartmentId = x.Department.Id,
                    DepartmentName = x.Department.Name,
                }
            }).FirstOrDefaultAsync();

            return Ok(user);
        }

        // PUT: api/user
        [HttpPut]
        [Route("edit_user")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> PutUserModel([FromBody] EditUserDTO editUserDTO)
        {

            if (!_context.Users.Any(x => x.Id == editUserDTO.UserId))
                return NotFound("User not found");

            var user = await _context.Users.FirstOrDefaultAsync(x => x.Id == editUserDTO.UserId);
            var department = _context.Users.Include(d => d.Department).FirstOrDefault(x => x.Id == _userManager.GetUserId(User)).Department;
            var currentUser = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

                _context.Users.FirstOrDefault(x => x.Id == user.Id).UserName = editUserDTO.Username;
                _context.Users.FirstOrDefault(x => x.Id == user.Id).FirstName = editUserDTO.FirstName;
                _context.Users.FirstOrDefault(x => x.Id == user.Id).LastName = editUserDTO.LastName;
                _context.Users.FirstOrDefault(x => x.Id == user.Id).Email = editUserDTO.Email;
                _context.Users.FirstOrDefault(x => x.Id == user.Id).UpdatedAt = DateTime.Now;
                await _context.SaveChangesAsync();

                var updatedUser = await _context.Users.Where(x => x.Id == user.Id).Select(x => new UserResponseDTO
                {
                    UserId = x.Id,
                    Username = x.UserName,
                    FirstName = x.FirstName,
                    LastName = x.LastName,
                    Email = x.Email,
                    Roles = x.Roles.Select(x => x.Role.Name).ToList()
                }).FirstOrDefaultAsync();

                return Ok(updatedUser);
        }

        // POST: api/user
        [HttpPost]
        [Route("reset_password")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> ResetPassword([FromBody] ResetPasswordDTO resetPasswordDTO)
        {
            if (!_context.Users.Any(x => x.Id == _userManager.GetUserId(User)))
                return NotFound("User not found");

            var user = await _userManager.FindByIdAsync(resetPasswordDTO.UserId);

            await _userManager.RemovePasswordAsync(user);
            await _userManager.AddPasswordAsync(user, resetPasswordDTO.NewPassword);

            return Ok("Password has successfully been reset");
        }

        // POST: api/user
        [HttpPost]
        [Route("add_department_user")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> AddDepartmentUser([FromBody] AddDepartmentUserDTO departmentUserDTO)
        {
            if (!_context.Users.Any(x => x.Id == departmentUserDTO.UserId))
                return NotFound("User not found");

            DepartmentModel department;

            if (departmentUserDTO.DepartmentId == 0)
            {
                department = await _context.Departments.FirstOrDefaultAsync(x => x.Users.Any(u => u.Id == _userManager.GetUserId(User)));
                if (department == null)
                    return BadRequest("You need to be in a department or send a DepartmentId along with the UserId");
            }
            else
                department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == departmentUserDTO.DepartmentId);

            if (!_context.Departments.Any(x => x.Id == department.Id))
                return NotFound("Department not found");

            var user = await _context.Users.Include(u => u.Department).Where(u => u.Id == departmentUserDTO.UserId).FirstOrDefaultAsync();

            if (user.Department != null)
                return Conflict("User already in " + user.Department.Name);

            user.Department = await _context.Departments.FirstOrDefaultAsync(x => x.Id == department.Id);
            await _userManager.UpdateAsync(user);
            return Ok("User added to: " + user.Department.Name);
        }

        // PUT: api/user
        [HttpPut]
        [Route("delete_department_user")]
        [Authorize(Roles = "Admin, Manager")]
        public async Task<IActionResult> DeleteDepartmentUser([FromBody] RemoveDepermentUserDTO removeDepermentUserDTO)
        {
            if (!_context.Users.Any(x => x.Id == removeDepermentUserDTO.UserId))
                return BadRequest();

            var user = await _context.Users.Include(d => d.Department).Where(x => x.Id == removeDepermentUserDTO.UserId).FirstOrDefaultAsync();
            var currentUser = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            if (currentUser.Id == user.Id)
                if (!user.Roles.Any(x => x.Role.Name == "Admin"))
                    return BadRequest();

            if (currentUser.Department.Id == user.Department.Id)
            {
                var departmentName = user.Department.Name;
                user.Department = null;
                await _userManager.UpdateAsync(user);
                return Ok("User removed from: " + departmentName);
            }
            else
                return NoContent();
        }

        // DELETE: api/User/5
        [HttpDelete("{id}")]
        [Route("delete_user")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> DeleteUserModel([FromQuery] GetUser user)
        {
            if (!_context.Users.Any(x => x.Id == user.UserId))
                return NotFound("User not found");


            var User = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == user.UserId);

            if (User.Department != null)
                return BadRequest("User still in department");

            if (User.Roles.Any(x => x.Role.Name == "Admin"))
                return BadRequest("Cant delete an admin");


            var username = User.UserName;
            await _userManager.DeleteAsync(User);

            await _context.SaveChangesAsync();

            return Ok($"User: {username} has been deleted");
        }
    }
}