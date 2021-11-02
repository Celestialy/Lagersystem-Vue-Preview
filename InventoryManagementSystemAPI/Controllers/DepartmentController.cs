using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using InventoryManagementSystemAPI.Database;
using InventoryManagementSystemAPI.Models;
using Microsoft.AspNetCore.Authorization;
using InventoryManagementSystemAPI.DTOs;
using Microsoft.AspNetCore.Identity;

namespace InventoryManagementSystemAPI.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DepartmentController : ControllerBase
    {
        private readonly DatabaseContext _context;
        private UserManager<UserModel> _userManager;

        public DepartmentController(DatabaseContext context, UserManager<UserModel> userManager)
        {
            _context = context;
            _userManager = userManager;
        }

        // GET: api/department
        [HttpGet]
        [Route("get_all_departments")]
        public async Task<IActionResult> GetDepartmentModel()
        {
            //var departments = await _context.Departments.Include(u => u.Users).Include(i => i.Inventories).Select(x => new DepartmentResponseDTO
            //{
            //    DepartmentId = x.Id,
            //    DepartmentName = x.Name
            //}).ToListAsync();

            var departments = await _context.Departments.Select(x => new DepartmentResponseDTO
            {
                DepartmentId = x.Id,
                DepartmentName = x.Name
            }).ToListAsync();

            //IEnumerable<DepartmentModel> departments = _context.Departments;

            await Task.Delay(1);

            if (departments.Count() >= 1)
                return Ok(departments);
            else
                return NotFound("No departments found");
        }

        // GET: api/department
        [HttpGet]
        [Route("get_specific_department")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> GetSingleDepartment([FromQuery] GetDepartmentDTO showDepartmentDTO)
        {
            if (!_context.Departments.Any(x => x.Id == showDepartmentDTO.DepartmentId))
                return NotFound("Department not found");

            var users = _context.Users.Where(x => x.Department.Id == showDepartmentDTO.DepartmentId).Select(x => new UserResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();

            var inventories = _context.Inventories.Include(i => i.Department).Where(x => x.Department.Id == showDepartmentDTO.DepartmentId).Select(x => new InventoryResponseDTO
            {
                InventoryId = x.Id,
                Name = x.Name,
                Address = x.Address,
                Zipcode = x.Zipcode,
                City = x.City,
                InventoryType = x.InventoryType
            }).ToList();

            var department = await _context.Departments.Where(d => d.Id == showDepartmentDTO.DepartmentId).Select(x => new DepartmentWithUsersAndInventoriesResponseDTO
            {
                DepartmentId = x.Id,
                DepartmentName = x.Name,
                Users = users,
                Inventories = inventories
            }).FirstOrDefaultAsync();

            return Ok(department);
        }

        // PUT: api/department
        [HttpPut]
        [Route("edit_department")]
        public async Task<IActionResult> PutDepartmentModel([FromBody] EditDepartmentDTO editDepartmentDTO)
        {
            if (!_context.Departments.Any(x => x.Id == editDepartmentDTO.DepartmentId))
                return NotFound("Department not found");

            var department = _context.Departments.Where(x => x.Id == editDepartmentDTO.DepartmentId);

            _context.Departments.FirstOrDefault(x => x.Id == editDepartmentDTO.DepartmentId).Name = editDepartmentDTO.NewDepartmentName;
            _context.Departments.FirstOrDefault(x => x.Id == editDepartmentDTO.DepartmentId).UpdatedAt = DateTime.Now;
            await _context.SaveChangesAsync();

            var users = _context.Users.Include(u => u.Department).Where(x => x.Department.Id == editDepartmentDTO.DepartmentId).Select(x => new UserResponseDTO
            {
                UserId = x.Id,
                Username = x.UserName,
                FirstName = x.FirstName,
                LastName = x.LastName,
                Email = x.Email
            }).ToList();

            var inventories = _context.Inventories.Include(u => u.Department).Where(x => x.Id == editDepartmentDTO.DepartmentId).Select(x => new InventoryResponseDTO
            {
                InventoryId = x.Id,
                Name = x.Name,
                Address = x.Address,
                Zipcode = x.Zipcode,
                City = x.City
            }).ToList();

            var departments = department.Where(x => x.Id == editDepartmentDTO.DepartmentId).Select(x => new DepartmentWithUsersAndInventoriesResponseDTO
            {
                DepartmentId = x.Id,
                DepartmentName = x.Name,
                Users = users,
                Inventories = inventories
            }).ToList();

            return Ok(departments);
        }

        // POST: api/department
        [HttpPost]
        [Route("add_department")]
        public async Task<IActionResult> PostDepartmentModel([FromBody] AddDepartmentDTO addDepartmentDTO)
        {
            if (_context.Inventories.Any(x => x.Name == addDepartmentDTO.DepartmentName))
                return BadRequest("Department already exists");

            DepartmentModel department = new DepartmentModel()
            {
                Name = addDepartmentDTO.DepartmentName,
                CreatedAt = DateTime.Now
            };

            _context.Departments.Add(department);
            await _context.SaveChangesAsync();

            return Ok($"Added department: {addDepartmentDTO.DepartmentName}");
        }

        // DELETE: api/department
        [HttpDelete()]
        [Route("delete_department")]
        public async Task<IActionResult> DeleteDepartmentModel([FromQuery] GetDepartmentDTO getDepartmentDTO)
        {
            if (!_context.Departments.Any(x => x.Id == getDepartmentDTO.DepartmentId))
                return NotFound("Department not found");

            if (_context.Users.Include(x => x.Department).Any(x => x.Department.Id == getDepartmentDTO.DepartmentId))
                return BadRequest("User connected to Department");

            if (_context.Inventories.Include(x => x.Department).Any(x => x.Department.Id == getDepartmentDTO.DepartmentId))
                return BadRequest("Inventory connected to Department");

            var department = _context.Departments.FirstOrDefault(x => x.Id == getDepartmentDTO.DepartmentId);

            _context.Remove(department);
            await _context.SaveChangesAsync();

            return Ok($"Deleted department: {department.Name}");
        }
    }
}