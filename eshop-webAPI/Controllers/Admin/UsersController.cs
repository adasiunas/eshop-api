using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Models;
using eshopAPI.Requests;
using Microsoft.AspNet.OData;
using Microsoft.AspNet.OData.Query;
using Microsoft.AspNet.OData.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using eshopAPI.DataAccess;

namespace eshopAPI.Controllers.Admin
{
    [Produces("application/json")]
    [Route("api/admin/users")]
    public class UsersController : ODataController
    {
        private UserManager<ShopUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private IShopUserRepository _userRepository;

        public UsersController(
            UserManager<ShopUser> userManager,
            ILogger<UsersController> logger,
            IShopUserRepository userRepository
            )
        {
            _userManager = userManager;
            _logger = logger;
            _userRepository = userRepository;
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