using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/feedback")]
    [AutoValidateAntiforgeryToken]
    [Authorize]
    public class UserFeedbackController:Controller
    {
        private IUserFeedbackRepository _repository;
        private UserManager<ShopUser> _userManager;
        private ILogger<UserFeedbackController> _logger;

        public UserFeedbackController(IUserFeedbackRepository repository, UserManager<ShopUser> userManager, ILogger<UserFeedbackController> logger)
        {
            _repository = repository;
            _userManager = userManager;
            _logger = logger;
        }
        
        
        [HttpPost]
        [Transaction]
        public async Task<IActionResult> Post([FromBody]UserFeedbackRequest request)
        {
            var user = await _userManager.FindByNameAsync(User.Identity.Name);

            if (user == null)
            {
                _logger.LogInformation($"User with name: {User.Identity.Name} was not found.");
                return StatusCode((int) HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "User was not found."));
            }
            
            var feedback = new UserFeedbackEntry
            {
                User = user,
                Message = request.Message,
                Rating = request.Rating
            };

            await _repository.Insert(feedback);
            
            return StatusCode((int) HttpStatusCode.NoContent);
        }
    }
}