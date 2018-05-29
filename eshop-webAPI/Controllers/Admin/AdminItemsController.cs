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
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
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

        [HttpGet("single")]
        public async Task<IActionResult> GetSingle([FromQuery] long id)
        {
            ItemVM item = await _itemRepository.FindItemVMByID(id);

            if(item == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Item with such ID was not found"));
            }

            return StatusCode((int)HttpStatusCode.OK, item);
        }

        [HttpPost("edit")]
        public async Task<IActionResult> UpdateItem([FromForm] ItemUpdateRequest request)
        {
            Item item = await _itemRepository.FindByID(request.ItemID);

            if(item == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Item with such ID was not found"));
            }

            SubCategory category = await _categoryRepository.FindSubCategoryByID(request.CategoryID);
            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Category was not found"));
            }

            List<string> pictureURLs = buildPictureUrlList(request.PictureURLs, request.PictureFiles);

            using (var transaction = await _attributeRepository.Context.Database.BeginTransactionAsync())
            {
                createNewAttributes(request.Attributes);

                Item modifiedItem = new Item()
                {
                    Description = request.Description,
                    Name = request.Name,
                    Price = request.Price,
                    SKU = request.SKU,
                    SubCategoryID = request.CategoryID,
                    Pictures = pictureURLs.Select(x => new ItemPicture() { URL = x }).ToList(),
                    Attributes = request.Attributes
                        ?.Select(x => new AttributeValue() { AttributeID = x.AttributeID, Value = x.Value })
                        ?.ToList(),
                    Timestamp = request.Force ? item.Timestamp : request.OptLockVersion
                };

                await _itemRepository.Update(item, modifiedItem);

                try
                {
                    await _itemRepository.SaveChanges();
                }
                catch(DbUpdateConcurrencyException ex)
                {
                    return StatusCode((int)HttpStatusCode.Conflict);
                }

                transaction.Commit();
            }

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPost("archive")]
        public async Task<IActionResult> Archive([FromBody] List<long> ids)
        {
            await _itemRepository.ArchiveByIDs(ids);
            await _itemRepository.SaveChanges();
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpPost("unarchive")]
        [Transaction]
        public async Task<IActionResult> Unarchive([FromBody] List<long> ids)
        {
            await _itemRepository.UnarchiveByIDs(ids);
            return StatusCode((int)HttpStatusCode.NoContent);
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
            
            List<string> pictureURLs = buildPictureUrlList(request.PictureURLs, request.PictureFiles);

            using (var transaction = await _attributeRepository.Context.Database.BeginTransactionAsync())
            {
                createNewAttributes(request.Attributes);

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

        private List<string> buildPictureUrlList(List<string> pictureUrls, List<IFormFile> pictureFiles)
        {
            List<string> tempList = new List<string>();

            if (pictureUrls != null)
                tempList.AddRange(pictureUrls);

            if (pictureFiles != null && pictureFiles.Count > 0)
            {
                List<Uri> uris = _imageCloudService.UploadImagesFromFiles(
                    pictureFiles
                        .Select(x => x.OpenReadStream())
                        .ToList()
                );
                tempList.AddRange(uris.Select(x => x.ToString()));
            }

            return tempList;
        }

        private async void createNewAttributes(List<AdminAttributeVM> attributes)
        {
            if (attributes == null)
                return;

            foreach (AdminAttributeVM vm in attributes.Where(x => x.AttributeID < 0).ToList())
            {
                Models.Attribute currentAttribute = await _attributeRepository.Insert(new Models.Attribute()
                {
                    Name = vm.Key
                });

                await _attributeRepository.SaveChanges();

                vm.AttributeID = currentAttribute.ID;
            }
        }
    }
}
