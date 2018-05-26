using eshopAPI.DataAccess;
using eshopAPI.Models.ViewModels;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Controllers.Admin
{
    public class AdminFeedbackController:ODataController
    {
        private IUserFeedbackRepository _feedbackRepository;
        public AdminFeedbackController(
            IUserFeedbackRepository repository)
        {
            _feedbackRepository = repository;
        }

        [HttpGet]
        [EnableQuery]
        public async Task<IQueryable<UserFeedbackVM>> Get()
        {
            return await _feedbackRepository.GetAllFeedbacksAsQueryable();
        }
    }
}
