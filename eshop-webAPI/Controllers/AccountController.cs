using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Helpers;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Services;
using eshopAPI.Utils;
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
        private readonly IShopUserRepository _shopUserRepository;
        private readonly UserManager<ShopUser> _userManager;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;

        // Add required services and they will be injected
        public AccountController(
            IShopUserRepository shopUserRepository,
            UserManager<ShopUser> userManager,
            SignInManager<ShopUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger)
        {
            _shopUserRepository = shopUserRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
        }

        [HttpGet("testconnection")]
        [AllowAnonymous]
        public IActionResult TestConnection()
        {
            return Ok();
        }

        [HttpGet("profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile()
        {
            ShopUserProfile profile = await _shopUserRepository.GetUserProfile(User.Identity.Name);
            return Ok(profile);
        }

        [HttpPost("register")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register([FromBody]RegisterRequest request)
        {
            var user = new ShopUser { UserName = request.Username, Email = request.Username };
            var result = await _userManager.CreateAsync(user, request.Password);
            if (result.Succeeded)
            {
                _logger.LogInformation("User created a new account with password.");

                await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                var confirmationLink = Url.EmailConfirmationLink(user.Id, code, Request.Scheme);
                await _emailSender.SendConfirmationEmailAsync(request.Username, confirmationLink);
                _logger.LogInformation($"Confirmation email was sent to user: {user.Name}");
                return Ok();
            }
            return BadRequest();
        }
        
        [HttpPost("login")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            _logger.LogInformation("Call to login from " + loginRequest.Email);
            
            var result = await _signInManager.PasswordSignInAsync(loginRequest.Email, loginRequest.Password,
                loginRequest.RememberMe, lockoutOnFailure: false);
            if (result.Succeeded)
            {
                _logger.LogInformation("User logged in.");
                return Ok();
            }

            return BadRequest("Can not log in");
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
        public async Task<IActionResult> ConfirmEmail(ConfirmUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogInformation($"User with id: {request.UserId} was not found.");
                return NotFound("User was not found");
            }
            var result = await _userManager.ConfirmEmailAsync(user, request.Code);
            if (result.Succeeded)
                return Ok("User is confirmed");

            return BadRequest();
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var shopUser = await _userManager.FindByEmailAsync(request.Email);
            
            if (shopUser != null)
            {
                var resetPasswordToken =  EncodeHelper.Base64Encode(await _userManager.GeneratePasswordResetTokenAsync(shopUser));
                var resetLink =
                    $"http://localhost:3000/resetpassword?Id={shopUser.Id}&token={resetPasswordToken}"; // TODO : make this configurable and redirectible to wep app
                await _emailSender.SendResetPasswordEmailAsync(request.Email, resetLink);
                return Ok("Password recovery confirmation link was sent to your e-mail.");
            }
            return NotFound("User with this email was not found");
        }
        
        // TODO : test this when UI is ready for reset password
        [HttpPost("resetpassword")]
        [AllowAnonymous]
        [ValidateAntiForgeryToken] //if request is made from email remove this
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var shopUser = await _userManager.FindByIdAsync(request.UserId);
            if (shopUser != null)
            {
                var decodedToken = EncodeHelper.Base64Decode(request.Token);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(shopUser, decodedToken, request.Password);
                if (resetPasswordResult.Succeeded)
                {
                    return Ok("Password was reset");
                }

                return BadRequest("Failed to reset password");
            }

            return NotFound("User was not found");
        }
    }
}