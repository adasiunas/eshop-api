using System.Collections.Generic;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.User;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    [Authorize]
    public class UserController : Controller
    {
        private readonly IShopUserRepository _shopUserRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(IShopUserRepository shopUserRepository,
            ILogger<UserController> logger)
        {
            _shopUserRepository = shopUserRepository;
            _logger = logger;
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
            var success = await _shopUserRepository.UpdateUserProfile(User.Identity.Name, updatedUser);
            if (success)
                return Ok("User data was updated");
            return BadRequest("User data could not be updated");
        }
    }
}
