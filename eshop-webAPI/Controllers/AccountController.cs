using Microsoft.AspNetCore.Mvc;

namespace eshop_webAPI.Controllers
{
    [Route("api/account")]
    public class AccountController : Controller
    {
        
        [HttpGet("profile")]
        public IActionResult Profile()
        {
            return Ok();
        }
        [HttpPost("register")]
        public IActionResult Register()
        {
            return Ok();
        }
        
        [HttpPost("login")]
        public IActionResult Login()
        {
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