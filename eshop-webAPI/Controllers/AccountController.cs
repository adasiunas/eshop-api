using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using eshopAPI.Models;
using eshopAPI.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace eshop_webAPI.Controllers
{    

    [Route("api/account")]
    public class AccountController : Controller
    {
        [Authorize]
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok(new User { ID = 0, Email = "test@test", Approved = true, Password = "Test", Role = UserRole.User});
        }

        [Authorize(Policy = "AdminRole")]
        [HttpGet("admin")]
        public IActionResult Admin()
        {
            return Ok(new User { ID = 0, Email = "test@test", Approved = true, Password = "Test", Role = UserRole.User });
        }

        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
        
        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Email, loginRequest.Username),
                new Claim(ClaimTypes.Role, "User")
            };
            var claimsIdentity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
            var claimsPrincipal = new ClaimsPrincipal(claimsIdentity);

            await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal: claimsPrincipal);
            
            return Ok();
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
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