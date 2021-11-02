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
    [Authorize(Roles = "Admin, Manager")]
    public class RoleUserController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;
        private RoleManager<RoleModel> _roleManager;

        public RoleUserController(DatabaseContext context, UserManager<UserModel> userManager, SignInManager<UserModel> signInManager, RoleManager<RoleModel> roleManager)
        {
            _context = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // GET: api/roleUser
        [HttpGet]
        [Route("get_users_with_specific_role")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetUsersWithSpecificRole([FromQuery] RoleDTO RoleDTO)
        {
            if (!await _roleManager.RoleExistsAsync(RoleDTO.Rolename))
                return NotFound("Role not found");

            var users = from _users in await _userManager.GetUsersInRoleAsync(RoleDTO.Rolename)
                        join derps in _context.Departments on _users.Department equals derps
                        select new UserResponseDTO
                        {
                            UserId = _users.Id,
                            Username = _users.UserName,
                            FirstName = _users.FirstName,
                            LastName = _users.LastName,
                            Email = _users.Email,
                            Roles = _users.Roles.Select(x => x.Role.Name).ToList()
                        };

            if (users.ToList().Count == 0)
                return NotFound("Users not found");

            return Ok(users);
        }

        // GET: api/roleUser
        [HttpGet]
        [Route("get_users_with_specific_role_in_department")]
        [Authorize(Roles = "Manager")]
        public async Task<IActionResult> GetUsersWithSpecificRoleInDepartment([FromQuery]RoleDTO RoleDTO)
        {
            if (!await _roleManager.RoleExistsAsync(RoleDTO.Rolename))
                return NotFound("Role not found");

            var user = await _context.Users.Include(d => d.Department).Where(x => x.Id == _userManager.GetUserId(User)).FirstOrDefaultAsync();

            var users = from _users in await _userManager.GetUsersInRoleAsync(RoleDTO.Rolename)
                        join derps in _context.Departments on _users.Department equals derps
                        where _users.Department.Id == user.Department.Id
                        select new UserResponseDTO
                        {
                            UserId = _users.Id,
                            Username = _users.UserName,
                            FirstName = _users.FirstName,
                            LastName = _users.LastName,
                            Email = _users.Email,
                            Roles = _users.Roles.Select(x => x.Role.Name).ToList()
                        };

            if (users.ToList().Count == 0)
                return NotFound("Users not found");

            return Ok(users);
        }

        // POST: api/roleUser
        [HttpPost]
        [Route("add_roleuser")]
        public async Task<IActionResult> PostRoleUserModel([FromBody] ManageRoleUserDTO addRoleUserDTO)
        {
            if (!_context.Users.Any(x => x.Id == addRoleUserDTO.UserId))
                return NotFound("User not found");
            else if (!await _roleManager.RoleExistsAsync(addRoleUserDTO.RoleName))
                return NotFound("Role not found");

            var user = await _userManager.FindByIdAsync(addRoleUserDTO.UserId);

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && addRoleUserDTO.RoleName == "Admin")
                return Unauthorized("Access denied");

                if (await _userManager.IsInRoleAsync(user, addRoleUserDTO.RoleName))
                return Conflict($"User already have the {addRoleUserDTO.RoleName} role");

            await _userManager.AddToRoleAsync(user, addRoleUserDTO.RoleName);
            return Ok($"Added {addRoleUserDTO.RoleName} to {user.UserName}");
        }

        // POST: api/roleUser
        [HttpPost]
        [Route("update_user_roles")]
        public async Task<IActionResult> UpdateRoles([FromBody] UpdateRolesDTO updateRoles)
        {
            if (!_context.Users.Any(x => x.Id == updateRoles.UserId))
                return NotFound("User not found");

            foreach (var item in updateRoles.Roles)
            {
                if (!await _roleManager.RoleExistsAsync(item))
                {
                    return NotFound(item + " not found");
                }
            }

            var user = await _userManager.FindByIdAsync(updateRoles.UserId);

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && updateRoles.Roles.Contains("Admin"))
                return Unauthorized("Access denied");

            foreach (var item in updateRoles.Roles)
            {
                if (!await _userManager.IsInRoleAsync(user, item))
                await _userManager.AddToRoleAsync(user, item);
            }

            List<string> rolesToDelete = new List<string>();

            foreach (var item in user.Roles)
            {
                if (!updateRoles.Roles.Contains(item.Role.Name))
                {
                    rolesToDelete.Add(item.Role.Name);
                }
            }
            await _userManager.RemoveFromRolesAsync(user, rolesToDelete);
            return Ok(updateRoles.Roles);
        }

        // DELETE: api/roleUser
        [HttpDelete("{id}")]
        [Route("delete_roleuser")]
        public async Task<IActionResult> DeleteRoleUserModel([FromQuery] ManageRoleUserDTO deleteRoleUserDTO)
        {
            if (!_context.Users.Any(x => x.Id == deleteRoleUserDTO.UserId))
                return NotFound("User not found");
            else if (!await _roleManager.RoleExistsAsync(deleteRoleUserDTO.RoleName))
                return NotFound("Role not found");

            var user = await _userManager.FindByIdAsync(deleteRoleUserDTO.UserId);

            var currentUser = await _userManager.GetUserAsync(User);
            if (!await _userManager.IsInRoleAsync(currentUser, "Admin") && deleteRoleUserDTO.RoleName == "Admin")
                return Unauthorized("Access denied");

            if (!await _userManager.IsInRoleAsync(user, deleteRoleUserDTO.RoleName))
                return Conflict($"User doesn't have the {deleteRoleUserDTO.RoleName} role");

            await _userManager.RemoveFromRoleAsync(user, deleteRoleUserDTO.RoleName);
            return Ok($"Removed {deleteRoleUserDTO.RoleName} from {user.UserName}");
        }
    }
}