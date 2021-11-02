using System;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using InventoryManagementSystemAPI.DTOs;
using InventoryManagementSystemAPI.Models;
using InventoryManagementSystemAPI.Database;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace InventoryManagementSystemAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AccountController : ControllerBase
    {
        private readonly DatabaseContext _context;
        readonly UserManager<UserModel> _userManager;
        readonly SignInManager<UserModel> signInManager;
        readonly IConfiguration configuration;
        readonly ILogger<AccountController> logger;

        public AccountController(
           DatabaseContext context,
           UserManager<UserModel> userManager,
           SignInManager<UserModel> signInManager,
           IConfiguration configuration,
           ILogger<AccountController> logger)
        {
            _context = context;
            this._userManager = userManager;
            this.signInManager = signInManager;
            this.configuration = configuration;
            this.logger = logger;
        }

        // GET: api/account
        [HttpGet]
        [Route("getCurrentUser")]
        public async Task<IActionResult> GetUserFromToken()
        {
            var user = await _context.Users.Include(d => d.Department).FirstOrDefaultAsync(x => x.Id == _userManager.GetUserId(User));

            //var roleNames = await _userManager.GetRolesAsync(user) as List<string>;
            //var roles = await _context.Roles.Where(x => roleNames.Any(z => z == x.Name)).ToListAsync();

            string departmentName = "";

            if (user.Department != null)
                departmentName = user.Department.Name;

            AccountDTO account = new AccountDTO
            {
                UserId = user.Id,
                Username = user.UserName,
                DepartmentName = departmentName,
                RoleNames = user.Roles.Select(x => x.Role.Name).ToList()

                //UserId = user.Id,
                //DepartmentId = user.Department.Id,
                //Roles = roles.Select(x => new AccountRoleDTO { RoleId = x.Id, RoleName = x.Name }).ToList<AccountRoleDTO>()
            };

            return Ok(account);
        }

        // PPOST: api/account
        [HttpPost]
        [AllowAnonymous]
        [Route("token")]
        public async Task<IActionResult> CreateToken([FromBody] LoginDTO loginModel)
        {
            
            if (ModelState.IsValid)
            {
                var loginResult = await signInManager.PasswordSignInAsync(loginModel.Username, loginModel.Password, isPersistent: false, lockoutOnFailure: false);

                if (!loginResult.Succeeded)
                    return BadRequest();

                var user = await _userManager.FindByNameAsync(loginModel.Username);

                return Ok(await GetToken(user));
            }

            return BadRequest(ModelState);
        }

        //[HttpPost]
        //[Authorize]
        //[Route("destroy_token")]
        //public async Task<IActionResult> DestroyToken()
        //{
        //    //TODO Fix Logout

        //    if (ModelState.IsValid)
        //    {
        //        var test = User.Claims;
        //        return Ok();
        //    }

        //    return BadRequest(ModelState);
        //}

        // POST: api/account
        [Authorize]
        [HttpPost]
        [Route("refresh_token")]
        public async Task<IActionResult> RefreshToken()
        {
            //gets current user
            var user = await _userManager.FindByNameAsync(
                User.Identity.Name ??
                User.Claims.Where(c => c.Properties.ContainsKey("unique_name")).Select(c => c.Value).FirstOrDefault()
                );

            return Ok(await GetToken(user));
        }

        // POST: api/account
        [HttpPost]
        [Route("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody] RegisterDTO registerModel)
        {
            if (!registerModel.Email.Contains("tcaa.dk"))
                if (!registerModel.Email.Contains("techcollege.dk"))
                    return BadRequest("Invalid Email");

            if (ModelState.IsValid)
            {
                var user = new UserModel
                {
                    //TODO: Use Automapper instaed of manual binding  

                    UserName = registerModel.Username,
                    FirstName = registerModel.FirstName,
                    LastName = registerModel.LastName,
                    Email = registerModel.Email,
                    CreatedAt = DateTime.Now
                };

                var identityResult = await this._userManager.CreateAsync(user, registerModel.Password);
                if (identityResult.Succeeded)
                {
                    await signInManager.SignInAsync(user, isPersistent: false);

                    await _userManager.AddToRoleAsync(user, "User");

                    return Ok(await GetToken(user));
                }
                else
                    return BadRequest(identityResult.Errors);
            }

            return BadRequest(ModelState);
        }

        // POST: api/account
        [HttpPost]
        [Route("change_password")]
        [Authorize]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordDTO changePasswordDTO)
        {
            var user = await _userManager.GetUserAsync(User);
            var result = _userManager.ChangePasswordAsync(user, changePasswordDTO.OldPassword, changePasswordDTO.NewPassword);
            
            if (!result.Result.Succeeded)
                return BadRequest("nuværende kode er forkert");

                return Ok("Password has successfully been changed");
        }

        private async Task<String> GetToken(UserModel user)
        {
            var utcNow = DateTime.UtcNow;
            var roleNames = await _userManager.GetRolesAsync(user);
            var claims = new List<Claim>
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
                new Claim(JwtRegisteredClaimNames.Iat, utcNow.ToString())
            };

            foreach (var role in roleNames)
            {
                claims.Add(new Claim(ClaimTypes.Role, role));
            }

            var signingKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(this.configuration.GetValue<String>("Tokens:Key")));
            var signingCredentials = new SigningCredentials(signingKey, SecurityAlgorithms.HmacSha256);
            var jwt = new JwtSecurityToken(
                signingCredentials: signingCredentials,
                claims: claims,
                notBefore: utcNow,
                expires: utcNow.AddSeconds(this.configuration.GetValue<int>("Tokens:Lifetime")),
                audience: this.configuration.GetValue<String>("Tokens:Audience"),
                issuer: this.configuration.GetValue<String>("Tokens:Issuer")
                );

            return new JwtSecurityTokenHandler().WriteToken(jwt);
        }
    }
}