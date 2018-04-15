using eshopAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
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

        [HttpGet]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Get()
        {
            var a = await _categoryRepository.GetAllCategoriesAsync();
            return Ok(a);
        }
    }
}
