using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
using eshopAPI.Requests.Categories;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
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
            var categories = await _categoryRepository.GetAllParentCategories();
            var categoryVMs = categories.Select(x => new AdminCategoryVM()
            {
                ID = x.ID,
                Name = x.Name,
                HasDescendants = x.SubCategories.Any()
            });
            return StatusCode((int) HttpStatusCode.OK, categoryVMs);
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

            var childrenVM = children.Select(x => new AdminCategoryVM()
            {
                ID = x.ID,
                Name = x.Name,
                HasDescendants = x.Items.Any()
            });

            return StatusCode((int) HttpStatusCode.OK, childrenVM);
        }

        [HttpPost("parent")]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> CreateCategory([FromBody] CategoryCreateRequest request)
        {
            Category category = await _categoryRepository.InsertCategory(new Category()
            {
                Name = request.Name
            });
            
            return StatusCode((int)HttpStatusCode.OK, category);
        }

        [HttpPost]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> CreateSubcategory([FromBody] SubcategoryCreateRequest request)
        {
            SubCategory subCategory = await _categoryRepository.InsertSubcategory(new SubCategory()
            {
                CategoryID = request.ParentID,
                Name = request.Name
            });
            
            return StatusCode((int)HttpStatusCode.OK, subCategory);
        }

        [HttpPut]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> UpdateSubcategoryName([FromBody] CategoryUpdateRequest request)
        {
            SubCategory subCategory = await _categoryRepository.FindSubCategoryByID(request.ID);

            if (subCategory == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Subcategory with such ID not found"));
            }

            subCategory.Name = request.Name;
            
            return StatusCode((int)HttpStatusCode.OK, subCategory);
        }

        [HttpPut("parent")]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> UpdateCategoryName([FromBody] CategoryUpdateRequest request)
        {
            Category category = await _categoryRepository.FindByID(request.ID);

            if (category == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Category with such ID not found"));
            }

            category.Name = request.Name;
            
            return StatusCode((int)HttpStatusCode.OK, category);
        }

        [HttpDelete]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> DeleteSubcategory([FromQuery] int id)
        {
            SubCategory subCategory = await _categoryRepository.FindSubCategoryByID(id);

            if (subCategory == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Subcategory with such ID not found"));
            }

            subCategory = await _categoryRepository.DeleteSubcategory(subCategory);
            
            return StatusCode((int)HttpStatusCode.OK, subCategory);
        }

        [HttpDelete("parent")]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> DeleteCategory([FromQuery] int id)
        {
            Category category = await _categoryRepository.FindByID(id);

            if(category == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound, 
                    new ErrorResponse(ErrorReasons.NotFound, "Category with such ID not found"));
            }

            category = await _categoryRepository.DeleteCategory(category);
            
            return StatusCode((int)HttpStatusCode.OK, category);
        }
    }
}
