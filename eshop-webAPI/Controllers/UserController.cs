using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.User;
using eshopAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/user")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class UserController : ODataController
    {
        private readonly IShopUserRepository _shopUserRepository;
        private readonly ILogger<UserController> _logger;

        public UserController(
            IShopUserRepository shopUserRepository,
            ILogger<UserController> logger)
        {
            _shopUserRepository = shopUserRepository;
            _logger = logger;
        }

        [HttpGet("profile")]
        public async Task<IActionResult> Profile()
        {
            ShopUserProfile profile = await _shopUserRepository.GetUserProfile(User.Identity.Name);
            return StatusCode((int) HttpStatusCode.OK, profile);
        }

        [HttpPut("updateUser")]
        [Transaction]
        public async Task<IActionResult> UpdateUser([FromBody] UpdateUserRequest updatedUser)
        {
            _logger.LogInformation($"Attempt to update profile of user with email ${User.Identity.Name}");
            ShopUser user = await _shopUserRepository.GetUserWithEmail(User.Identity.Name);
            
            if (user == null)
            {
                _logger.LogInformation("Attempt to update user profile failed, user with such email not found");
                return StatusCode((int) HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
            }

            await _shopUserRepository.UpdateUserProfile(user, updatedUser);
            
            _logger.LogInformation("User profile successfully updated");
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}
