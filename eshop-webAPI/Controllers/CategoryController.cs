using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Category")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(ILogger<CategoryController> logger, ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        //[EnableQuery]
        //[HttpGet]
        //[AllowAnonymous]
        //public IEnumerable<Category> Get()
        //{
        //    //return _categoryRepository.GetAllCategories();
        //}
    }
}