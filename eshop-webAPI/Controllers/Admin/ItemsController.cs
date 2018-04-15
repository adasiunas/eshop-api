using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers.Admin
{
    [Route("api/admin/items")]
    [AutoValidateAntiforgeryToken]
    public class ItemsController : Controller
    {
        private IItemRepository _itemRepository;
        private ILogger<ItemsController> _logger;
        private ICategoryRepository _categoryRepository;

        public ItemsController(
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
        public IQueryable<AdminItemVM> Get()
        {
            return _itemRepository.GetAllAdminItemVMAsQueryable();
        }

        [HttpPost("create")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Create([FromBody]ItemCreateRequest request)
        {
            Category category = await _categoryRepository.FindByIDAsync(request.CategoryID);

            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return NotFound();
            }

            await _itemRepository.InsertAsync(new Item()
            {
                CreateDate = DateTime.Now,
                Description = request.Description,
                Name = request.Name,
                Price = request.Price,
                SKU = request.SKU,
                Category = category
            });

            return Ok();
        }

    }
}
