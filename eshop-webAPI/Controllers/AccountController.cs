using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Services;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshop_webAPI.Controllers
{    

    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly IUserService _userService;
        private readonly ShopContext _context;
        private readonly ILogger<AccountController> _logger;

        // Add required services and they will be injected
        public AccountController(IUserService userService, ShopContext context, ILogger<AccountController> logger)
        {
            _userService = userService;
            _context = context;
            _logger = logger;
        }

        [Authorize(Policy = "User")]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            string userId = HttpContext.User.FindFirstValue(ClaimTypes.PrimarySid);
            if (null == userId)
                return BadRequest();

            long id = long.Parse(userId);
            User user = _context.Users.First(u => u.ID == id);
            return Ok(user);
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            _logger.LogInformation("Call to login from " + loginRequest.Username);

            User user = _userService.ValidateUser(loginRequest.Username, loginRequest.Password);
            if (null == user)
                return BadRequest();

            string userRole = _userService.GetUserRole(user);
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, loginRequest.Username),
                new Claim(ClaimTypes.Role, userRole),
                new Claim(ClaimTypes.PrimarySid, user.ID.ToString())
            };

            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal: claimsPrincipal);

            _logger.LogInformation("Logged in successfuly");
            return Ok();
        }
        
        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
            return Ok();
        }
        
        [HttpPost("varify/email")]
        public IActionResult VarifyEmail()
        {
            return Ok();
        }
        
        [HttpPost("reset/password")]
        public IActionResult ResetPassword()
        {
            return Ok();
        }
        
        [HttpPost("remember")]
        public IActionResult RememberPassword()
        {
            return Ok();
        }
    }
}