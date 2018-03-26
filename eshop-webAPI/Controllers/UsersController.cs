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

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Users")]
    public class UsersController : ODataController
    {
        private UserManager<ShopUser> _userManager;
        private readonly ILogger<UsersController> _logger;
        private IUserRepository _userRepository;

        public UsersController(
            UserManager<ShopUser> userManager,
            ILogger<UsersController> logger,
            IUserRepository userRepository
            )
        {
            _userManager = userManager;
            _logger = logger;
            _userRepository = userRepository;
        }

        List<ShopUser> list = new List<ShopUser>()
        {
            new ShopUser()
            {
                Id = "1",
                Email = "hahaha@hahaha.com",
                Name = "Tomas Burkus"
            },
            new ShopUser()
            {
                Id = "2",
                Email = "lohaha@hahaha.com",
                Name = "Lomas Murkus",
            },
            new ShopUser()
            {
                Id = "3",
                Email = "zozo@hahaha.com",
                Name = "Mantas Jurkus"
            }
        };

        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<UserVM>> Get()
        {
            List<UserVM> userModels = _userRepository.GetAll();
            
            return userModels.AsQueryable();
        }

        [HttpPost("changerole")]
        public async Task<IActionResult> ChangeRole([FromBody]RoleChangeRequest request)
        {
            _logger.LogInformation($"Changing role of user with email ${request.Email} to ${request.Role}");

            ShopUser user = await _userManager.FindByEmailAsync(request.Email);

            if (user == null)
            {
                _logger.LogInformation($"Role changing failed, no user with such email found");
                return BadRequest();
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