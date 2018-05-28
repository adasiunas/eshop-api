using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels.Admin;
using eshopAPI.Requests;
using eshopAPI.Services;
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
        private IImageCloudService _imageCloudService;
        private IAttributeRepository _attributeRepository;
        private ShopContext _shopContext;

        public AdminItemsController(
            ILogger<ItemsController> logger,
            IItemRepository itemRepository,
            ICategoryRepository categoryRepository,
            IAttributeRepository attributeRepository,
            IImageCloudService imageCloudService,
            ShopContext shopContext)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _imageCloudService = imageCloudService;
            _attributeRepository = attributeRepository;
            _shopContext = shopContext;
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
        public async Task<IActionResult> Create([FromForm]ItemCreateRequest request)
        {
            _logger.LogInformation($"Creating a new item with SKU: {request.SKU}");

            SubCategory category = await _categoryRepository.FindSubCategoryByID(request.CategoryID);
            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Category was not found"));
            }

            List<string> pictureURLs = new List<string>();

            if (request.PictureURLs != null)
                pictureURLs.AddRange(request.PictureURLs);

            if (request.PictureFiles != null && request.PictureFiles.Count > 0)
            {
                List<Uri> uris = _imageCloudService.UploadImagesFromFiles(
                    request.PictureFiles
                        .Select(x => x.OpenReadStream())
                        .ToList()
                );
                pictureURLs.AddRange(uris.Select(x => x.ToString()));
            }

            using (var transaction = await _attributeRepository.Context.Database.BeginTransactionAsync())
            {
                foreach (AdminAttributeVM vm in request.Attributes.Where(x => x.AttributeID < 0).ToList())
                {
                    Models.Attribute currentAttribute = await _attributeRepository.Insert(new Models.Attribute()
                    {
                        Name = vm.Key
                    });

                    await _attributeRepository.SaveChanges();

                    vm.AttributeID = currentAttribute.ID;
                }

                await _itemRepository.Insert(new Item()
                {
                    Description = request.Description,
                    Name = request.Name,
                    Price = request.Price,
                    SKU = request.SKU,
                    SubCategoryID = request.CategoryID,
                    Pictures = pictureURLs.Select(x => new ItemPicture() { URL = x }).ToList(),
                    Attributes = request.Attributes
                        ?.Select(x => new AttributeValue() { AttributeID = x.AttributeID, Value = x.Value })
                        ?.ToList()
                });

                await _itemRepository.SaveChanges();

                transaction.Commit();
            }
            return StatusCode((int)HttpStatusCode.NoContent);
        }

    }
}
