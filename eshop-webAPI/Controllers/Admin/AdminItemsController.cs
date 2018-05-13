using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers.Admin
{
    [Route("api/admin/Items")]
    [AutoValidateAntiforgeryToken]
    public class AdminItemsController : ODataController
    {
        private IItemRepository _itemRepository;
        private ILogger<ItemsController> _logger;
        private ICategoryRepository _categoryRepository;

        public AdminItemsController(
            ILogger<ItemsController> logger,
            IItemRepository itemRepository,
            ICategoryRepository categoryRepository)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
        }

        // GET: api/Items
        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<AdminItemVM>> Get()
        {
            return await _itemRepository.GetAllAdminItemVMAsQueryable();
        }

        [HttpPost("create")]
        [IgnoreAntiforgeryToken]
        [Transaction]
        public async Task<IActionResult> Create([FromBody]ItemCreateRequest request)
        {
            SubCategory category = await _categoryRepository.FindSubCategoryByID(request.CategoryID);

            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return StatusCode((int) HttpStatusCode.NotFound, 
                    new ErrorResponse(ErrorReasons.NotFound, "Category was not found"));
            }

            await _itemRepository.Insert(new Item()
            {
                Description = request.Description,
                Name = request.Name,
                Price = request.Price,
                SKU = request.SKU,
                SubCategoryID = request.CategoryID
            });
            
            return StatusCode((int) HttpStatusCode.NoContent);
        }

    }
}
