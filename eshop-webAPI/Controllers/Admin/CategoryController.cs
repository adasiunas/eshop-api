using eshopAPI.DataAccess;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eshopAPI.Controllers.Admin
{
    [Route("api/admin/categories")]
    public class CategoryController : Controller
    {
        private readonly ILogger<CategoryController> _logger;
        private readonly ICategoryRepository _categoryRepository;

        public CategoryController(
            ILogger<CategoryController> logger,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _categoryRepository = categoryRepository;
        }

        [HttpGet("parent")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Get()
        {
            var categories = await _categoryRepository.GetAllParentCategoriesAsync();
            return StatusCode((int) HttpStatusCode.OK, categories);
        }

        [HttpGet("{parentId}")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetChildren(int parentId)
        {
            var children = await _categoryRepository.GetChildrenOfParent(parentId);

            if(children == null)
            {
                _logger.LogError("No subcategories found for category - " + parentId);
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "No subcategories found"));
            }

            return StatusCode((int) HttpStatusCode.OK, children);
        }
    }
}
