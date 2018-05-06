using System.Linq;
using System.Net;
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
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshop_webAPI.Controllers
{
    [Produces("application/json")]
    [Authorize]
    [Route("api/account")]
    [AutoValidateAntiforgeryToken]
    public class AccountController : Controller
    {
        private readonly IShopUserRepository _shopUserRepository;
        private readonly UserManager<ShopUser> _userManager;
        private readonly SignInManager<ShopUser> _signInManager;
        private readonly ILogger<AccountController> _logger;
        private readonly IEmailSender _emailSender;
        private readonly IConfiguration _configuration;

        // Add required services and they will be injected
        public AccountController(
            IShopUserRepository shopUserRepository,
            UserManager<ShopUser> userManager,
            SignInManager<ShopUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            IConfiguration configuration)
        {
            _shopUserRepository = shopUserRepository;
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _configuration = configuration;
        }

        [HttpGet("renewcsrftoken")]
        [AllowAnonymous]
        public IActionResult RenewCsrfToken()
        {
            return NoContent();
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

                await _userManager.AddToRoleAsync(user, UserRole.User.ToString());
                var code = EncodeHelper.Base64Encode(await _userManager.GenerateEmailConfirmationTokenAsync(user));
                var confirmationLink = UrlExtensions.EmailConfirmationLink(user.Id, code, _configuration["RedirectDomain"]);
                await _emailSender.SendConfirmationEmailAsync(request.Username, confirmationLink);
                _logger.LogInformation($"Confirmation email was sent to user: {user.Name}");
                return StatusCode((int) HttpStatusCode.NoContent);
            }

            var errorResponse = new ErrorResponse(ErrorReasons.BadRequest, result.Errors.Select(e => e.Description).FirstOrDefault());
            return StatusCode((int) HttpStatusCode.BadRequest, errorResponse);
        }

        [HttpPost("login")]
        [AllowAnonymous]
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            _logger.LogInformation("Call to login from " + loginRequest.Email);
            
            if (_signInManager.IsSignedIn(HttpContext.User))
            {
                _logger.LogInformation("User is already signed in.");
                return StatusCode((int) HttpStatusCode.NoContent);
            }

            var user = await _userManager.FindByEmailAsync(loginRequest.Email);
            
            if (user == null)
            {
                return StatusCode((int) HttpStatusCode.BadRequest,
                    new ErrorResponse(ErrorReasons.InvalidEmailOrPassword, "Invalid email or password."));
            }

            if (!user.EmailConfirmed)
            {
                return StatusCode((int) HttpStatusCode.BadRequest,
                    new ErrorResponse(ErrorReasons.EmailNotConfirmed, "Please confirm your account"));
            }
            
            var result = await _signInManager.PasswordSignInAsync(user, loginRequest.Password,
                loginRequest.RememberMe, lockoutOnFailure: false);

            if (!result.Succeeded)
                return StatusCode((int) HttpStatusCode.BadRequest,
                    new ErrorResponse(ErrorReasons.BadRequest, "Failed to log in. Please make sure you have entered correct credentials."));
            
            _logger.LogInformation("User logged in.");
            return StatusCode((int) HttpStatusCode.NoContent);

        }

        [HttpPost("logout")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Logout()
        {
            if (!_signInManager.IsSignedIn(User))
            {
                _logger.LogInformation("User is not signed in so it cannot be signed out");
                return StatusCode((int)HttpStatusCode.NoContent);
            }
            await _signInManager.SignOutAsync();
            _logger.LogInformation("User logged out.");
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("confirmaccount")]
        [AllowAnonymous]
        public async Task<IActionResult> ConfirmEmail(ConfirmUserRequest request)
        {
            var user = await _userManager.FindByIdAsync(request.UserId);
            if (user == null)
            {
                _logger.LogInformation($"User with id: {request.UserId} was not found.");
                return StatusCode((int) HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
            }
            var result = await _userManager.ConfirmEmailAsync(user, EncodeHelper.Base64Decode(request.Code));
            if (result.Succeeded)
                return StatusCode((int)HttpStatusCode.OK, "User confirmed");

            return StatusCode((int) HttpStatusCode.BadRequest,
                new ErrorResponse(ErrorReasons.BadRequest, ErrorReasons.BadRequest));
        }

        [HttpPost("forgotPassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ForgotPassword([FromBody] ForgotPasswordRequest request)
        {
            var shopUser = await _userManager.FindByEmailAsync(request.Email);

            if (shopUser != null)
            {
                var resetPasswordToken = EncodeHelper.Base64Encode(await _userManager.GeneratePasswordResetTokenAsync(shopUser));
                var resetLink = UrlExtensions.ResetPasswordLink(shopUser.Id, resetPasswordToken,
                    _configuration["RedirectDomain"]);
                await _emailSender.SendResetPasswordEmailAsync(request.Email, resetLink);
                return StatusCode((int)HttpStatusCode.OK, "Password recovery confirmation link was sent to your e-mail.");
            }
            
            return StatusCode((int) HttpStatusCode.NotFound,
                new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
        }

        [HttpPost("resetpassword")]
        [AllowAnonymous]
        public async Task<IActionResult> ResetPassword([FromBody]ResetPasswordRequest request)
        {
            var shopUser = await _userManager.FindByIdAsync(request.UserId);
            if (shopUser != null)
            {
                var decodedToken = EncodeHelper.Base64Decode(request.Token);
                var resetPasswordResult = await _userManager.ResetPasswordAsync(shopUser, decodedToken, request.Password);
                if (resetPasswordResult.Succeeded)
                {
                    return StatusCode((int) HttpStatusCode.OK, "Password was reset");
                }
                
                return StatusCode((int) HttpStatusCode.BadRequest,
                    new ErrorResponse(ErrorReasons.BadRequest,
                        resetPasswordResult.Errors.Select(e => e.Description).FirstOrDefault()));
            }

            return StatusCode((int) HttpStatusCode.NotFound,
                new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
        }

        [HttpPut("changepassword")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ChangePassword([FromBody] ChangePasswordRequest request)
        {
            _logger.LogInformation($"Attempt to change password for user with email ${User.Identity.Name}");
            ShopUser user = await _userManager.FindByEmailAsync(User.Identity.Name);

            if (user == null)
            {
                _logger.LogInformation("User with such email not found");
                return StatusCode((int) HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
            }

            var result = await _userManager.ChangePasswordAsync(user, request.CurrentPassword, request.NewPassword);

            if(result.Succeeded)
            {
                await _signInManager.SignOutAsync();
                _logger.LogInformation($"User {User.Identity.Name} has been signed out");
                _logger.LogInformation("Password changed successfully");
                return StatusCode((int)HttpStatusCode.NoContent);
            }

            _logger.LogInformation($"Attempt to change password failed: {result.Errors.Select(e => e.Description)}");
            return StatusCode((int) HttpStatusCode.BadRequest,
                new ErrorResponse(ErrorReasons.BadRequest,
                    result.Errors.Select(e => e.Description).FirstOrDefault()));
        }
    }
}