using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Requests.User;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    //[Authorize]
    public class UserController : ODataController
    {
        private readonly IShopUserRepository _shopUserRepository;
        private readonly ILogger<UserController> _logger;
        private IUserRepository _userRepository;
        private UserManager<ShopUser> _userManager;

        public UserController(
            IShopUserRepository shopUserRepository,
            ILogger<UserController> logger,
            UserManager<ShopUser> userManager,
            IUserRepository userRepository)
        {
            _shopUserRepository = shopUserRepository;
            _logger = logger;
            _userManager = userManager;
            _userRepository = userRepository;
        }

        [HttpGet("profile")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Profile()
        {
            ShopUserProfile profile = await _shopUserRepository.GetUserProfile(User.Identity.Name);
            return Ok(profile);
        }

        [HttpPut("updateUser")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updatedUser)
        {
            _logger.LogInformation($"Attempt to update profile of user with email ${User.Identity.Name}");
            ShopUser user = await _shopUserRepository.GetUserWithEmail(User.Identity.Name);
            if (user == null)
            {
                _logger.LogInformation("Attempt to update user profile failed, user with such email not found");
                return NotFound();
            }

            var success = await _shopUserRepository.UpdateUserProfile(user, updatedUser);
            if (success)
            {
                _logger.LogInformation("User profile successfully updated");
                return Ok();
            }
            _logger.LogInformation("User profile could not be updated");
            return BadRequest("User profile could not be updated");
        }

        [EnableQuery]
        [HttpGet]
        public IQueryable<UserVM> Get()
        {
            return _userRepository.GetAllUsersAsQueryable();
        }

        [HttpPost("changerole")]
        public async Task<IActionResult> ChangeRole([FromBody]RoleChangeRequest request)
        {
            _logger.LogInformation($"Changing role of user with email ${request.Email} to ${request.Role}");

            ShopUser user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogInformation($"Role changing failed, no user with such email found");
                return NotFound();
            }

            try
            {
                UserRole role = (UserRole)Enum.Parse(typeof(UserRole), request.Role);
            }
            // happens if role string cannot be parsed
            catch (ArgumentException e)
            {
                _logger.LogInformation($"Role changing failed, bad role provided");
                return BadRequest();
            }

            IList<string> roles = await _userManager.GetRolesAsync(user);
            await _userManager.RemoveFromRolesAsync(user, roles);

            await _userManager.AddToRoleAsync(user, request.Role);

            _logger.LogInformation($"Role succesfully changed");
            return Ok();
        }
    }
}
