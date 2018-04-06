﻿using System;
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
        private UserManager<ShopUser> _userManager;

        public UserController(
            IShopUserRepository shopUserRepository,
            ILogger<UserController> logger,
            UserManager<ShopUser> userManager)
        {
            _shopUserRepository = shopUserRepository;
            _logger = logger;
            _userManager = userManager;
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
            return _shopUserRepository.GetAllUsersAsQueryable();
        }

    }
}
