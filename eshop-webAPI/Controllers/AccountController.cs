using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Helpers;
using eshopAPI.Models;
using eshopAPI.Requests;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshop_webAPI.Controllers
{    
    [Authorize]
    [Route("api/account")]
    public class AccountController : Controller
    {
        private readonly UserManager<ShopUser> _userManager;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly ILogger<AccountController> _logger;

        // Add required services and they will be injected
        public AccountController(
            UserManager<ShopUser> userManager,
            SignInManager<ShopUser> signInManager,
            ILogger<AccountController> logger)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _logger = logger;
        }

        [HttpGet("profile")]
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Profile()
        {
            ShopUser user = await _userManager.FindByNameAsync(User.Identity.Name);
            return Ok(user.GetUserProfile());
        }


        [HttpPost("register")]
        [AllowAnonymous]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request)
        {
            var user = new ShopUser { UserName = request.Username, Email = request.Username };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var callbackUrl = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                // need to send email with confirmation link
                //await _emailSender.SendEmailConfirmationAsync(request.Username, callbackUrl);

                await _signInManager.SignInAsync(user, isPersistent: false);
                _logger.LogInformation("User created a new account with password.");
                return Ok();
            }
            return BadRequest();
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            _logger.LogInformation("Call to login from " + loginRequest.Username);

            var result = await _signInManager.PasswordSignInAsync(loginRequest.Username, loginRequest.Password,
                loginRequest.RememberMe, lockoutOnFailure: false);

            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok();
            }

            return BadRequest("Cannot login");
        }
        
        [HttpPost("logout")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Logout()
        {
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return Ok();
        }

        [HttpGet]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(string userId, string code)
        {
            if (userId == null || code == null)
            {
                return BadRequest();
            }
            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return BadRequest();
            }
            var result = await _userManager.ConfirmEmailAsync(user, code);
            if (result.Succeeded)
                return Ok();

            return BadRequest();
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