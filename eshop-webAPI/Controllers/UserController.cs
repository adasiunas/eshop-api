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

        [HttpPut("updateAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateAddress([FromBody] AddressRequest addressRequest)
        {
            var success = await _shopUserRepository.UpdateUserAddress(User.Identity.Name, addressRequest);
            if (success)
                return Ok("User address was updated");
            return BadRequest("Address could not be updated");
        }

        [HttpDelete("deleteAddress/{id}")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteAddress(long id)
        {
            var success = await _shopUserRepository.DeleteAddress(User.Identity.Name, id);
            if (success)
                return Ok("Address deleted");
            return BadRequest("Address could not be deleted");
        }

        [HttpPost("addAddress")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddAddress([FromBody] AddressRequest newAddress)
        {
            var success = await _shopUserRepository.AddAddress(User.Identity.Name, newAddress);
            if (success)
                //ar nereikia cia 201 grazint?
                return Ok("Address added");
            return BadRequest("Address could not be added");
        }
    }
}
