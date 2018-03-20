using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Threading.Tasks;
using eshopAPI.Helpers;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Services;
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
        private readonly IEmailSender _emailSender;
        private readonly IUserClaimsService _userClaimsService;
        private readonly ITokenGenerator _tokenGenerator;

        // Add required services and they will be injected
        public AccountController(
            UserManager<ShopUser> userManager,
            SignInManager<ShopUser> signInManager,
            IEmailSender emailSender,
            ILogger<AccountController> logger,
            ITokenGenerator tokenGenerator,
            IUserClaimsService userClaimsService)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _emailSender = emailSender;
            _logger = logger;
            _tokenGenerator = tokenGenerator;
            _userClaimsService = userClaimsService;
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
        public async Task<IActionResult> Login([FromBody]LoginRequest loginRequest)
        {
            _logger.LogInformation("Call to login from " + loginRequest.Email);

            ShopUser user = await _userManager.FindByEmailAsync(loginRequest.Email);
            var result = await _signInManager.CheckPasswordSignInAsync(user, loginRequest.Password, false);

            if (result.Succeeded)
            {
                IEnumerable<Claim> claims = await _userClaimsService.GetUserClaims(user);
                JwtSecurityToken token = _tokenGenerator.GenerateToken(claims);

                _logger.LogInformation("User logged in.");
                return Ok(new { token = new JwtSecurityTokenHandler().WriteToken(token) });
            }

            return BadRequest("Can not log in");
        }
        
        [HttpPost("logout")]
        public IActionResult Logout()
        {
            // thinks something how to handle logout
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