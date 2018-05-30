using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Http.Headers;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels.Admin;
using eshopAPI.Requests;
using eshopAPI.Response;
using eshopAPI.Services;
using eshopAPI.Utils;
using eshopAPI.Utils.Export;
using eshopAPI.Utils.Import;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/Items")]
    [AutoValidateAntiforgeryToken]
    public class AdminItemsController : ODataController
    {
        private IItemRepository _itemRepository;
        private IAttributeRepository _attributeRepository;
        private ILogger<ItemsController> _logger;
        private ICategoryRepository _categoryRepository;
        private ImportErrorLogger _importErrorLogger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;
        private IImageCloudService _imageCloudService;
        private readonly ShopContext _shopContext;

        public AdminItemsController(
            ILogger<ItemsController> logger,
            IItemRepository itemRepository,
            ICategoryRepository categoryRepository,
            IAttributeRepository attributeRepository,
            IHostingEnvironment hostingEnvironment, 
            IConfiguration configuration, 
            IExportService exportService,
            IImportService importService,
            IImageCloudService imageCloudService,
            ShopContext shopContext)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _exportService = exportService;
            _importService = importService;
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

            Category category = await _categoryRepository.FindByID(request.CategoryID);
            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Category was not found"));
            }

            if (request.SubcategoryID.HasValue)
            {
                SubCategory subCategory = await _categoryRepository.FindSubCategoryByID(request.SubcategoryID.Value);
                if (subCategory == null)
                {
                    _logger.LogInformation($"No subcategory found with id {request.SubcategoryID.Value}");
                    return StatusCode((int)HttpStatusCode.NotFound,
                        new ErrorResponse(ErrorReasons.NotFound, "Subcategory was not found"));
                }
            }

            List<string> pictureURLs = BuildPictureUrlList(request.PictureURLs, request.PictureFiles);

            using (var transaction = await _attributeRepository.Context.Database.BeginTransactionAsync())
            {
                await CreateNewAttributes(request.Attributes);

                Item modifiedItem = new Item()
                {
                    Description = request.Description,
                    Name = request.Name,
                    Price = request.Price,
                    SKU = request.SKU,
                    CategoryID = request.CategoryID,
                    SubCategoryID = request.SubcategoryID,
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

            Category category = await _categoryRepository.FindByID(request.CategoryID);
            if (category == null)
            {
                _logger.LogInformation($"No category found with id {request.CategoryID}");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Category was not found"));
            }

            if (request.SubcategoryID.HasValue)
            {
                SubCategory subCategory = await _categoryRepository.FindSubCategoryByID(request.SubcategoryID.Value);
                if (subCategory == null)
                {
                    _logger.LogInformation($"No subcategory found with id {request.SubcategoryID.Value}");
                    return StatusCode((int)HttpStatusCode.NotFound,
                        new ErrorResponse(ErrorReasons.NotFound, "Subcategory was not found"));
                }
            }
            
            List<string> pictureURLs = BuildPictureUrlList(request.PictureURLs, request.PictureFiles);

            using (var transaction = await _attributeRepository.Context.Database.BeginTransactionAsync())
            {
                await CreateNewAttributes(request.Attributes);

                await _itemRepository.Insert(new Item()
                {
                    Description = request.Description,
                    Name = request.Name,
                    Price = request.Price,
                    SKU = request.SKU,
                    CategoryID = request.CategoryID,
                    SubCategoryID = request.SubcategoryID,
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

        [HttpGet("export/{categoryId}")]
        public async Task<IActionResult> Export(int categoryId)
        {
            IEnumerable<ItemVM> items = await _itemRepository.GetAllItemsForFirstPage();
            items = items.Where(i => i.Category.ID == categoryId);
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items for this category was found."));
            }

            return await PerformExport(items);
        }

        [HttpGet("exportAll")]
        public async Task<IActionResult> ExportAll()
        {
            
            IEnumerable<ItemVM> items = await _itemRepository.GetAllItemsForFirstPage();
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items was found."));
            }
            return await PerformExport(items);
                
        }
        
        
        [HttpGet("export/subcategory/{subcategoryId}")]
        public async Task<IActionResult> ExportSubcategory(int subcategoryId)
        {
            IEnumerable<ItemVM> items = await _itemRepository.GetAllItemsForFirstPage();
            items = items.Where(i => i.SubCategory != null && i.SubCategory.ID == subcategoryId);
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items for this subcategory was found."));
            }

            return await PerformExport(items);
        }

        [HttpGet("export/file/{fileName}")]
        public async Task<IActionResult> DownloadExportedFile(string fileName)
        {
            if (string.IsNullOrEmpty(fileName))
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.BadRequest, "Incorrect file name"));
            }
            
            var exportFileDirectory = !string.IsNullOrEmpty(_configuration["ExportedFilesDirectory"]) ? Path.Combine(_configuration["ExportedFilesDirectory"], fileName) : Path.Combine(_hostingEnvironment.ContentRootPath, "ExportedFiles", fileName);
            byte[] bytes;
            using (var fileStream = new FileStream(exportFileDirectory, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fileStream.Length];
                int numBytesToRead = (int)fileStream.Length;
                int numBytesRead = 0;
                while (numBytesToRead > 0)
                {
                    int n = await fileStream.ReadAsync(bytes, numBytesRead, numBytesToRead);
                    if (n == 0) break;
                    numBytesRead += n;
                    numBytesToRead -= n;
                }
            }
            Response.Headers.Add("content-disposition", $"attachment;filename=\"{fileName}\"");
            return File(bytes, "application/octet-stream");
        }

        [HttpGet("import")]
        public async Task<IActionResult> Import()
        {
            //var files = Request.Form.Files;
            //foreach (var file in files)
            //{
            //    var filename = ContentDispositionHeaderValue
            //                    .Parse(file.ContentDisposition)
            //                    .FileName
            //                    .Trim('"');
            //    filename = hostingEnv.WebRootPath + $@"\{filename}";
            //    using (FileStream fs = System.IO.File.Create(filename))
            //    {
            //        file.CopyTo(fs);
            //        fs.Flush();
            //    }
            //}

            _importErrorLogger = new ImportErrorLogger(_logger);
            _importService.SetFileName("items");
            _importService.ImportErrorLogger = _importErrorLogger;

            var importedItems = await _importService.ImportItems();

            if (importedItems == null)
            {
                return NoContent();
            }

            var savedItems = new List<ItemVM>();
            var skuCodes = (await _itemRepository.GetAllItemsSkuCodes()).ToList();

            for (int i = 0; i < importedItems.Count(); i++)
            {
                var item = importedItems.ElementAt(i);
                var row = i + 2;

                Category category = await _categoryRepository.FindByName(item.Category.Name);
                if (category == null)
                {
                    _importErrorLogger.LogError(row, $"Category {item.Category.Name} does not exist");
                    continue;
                }

                var subcategories = await _categoryRepository.GetChildrenOfParent(category.ID);
                long subcategoryId = subcategories.Where(x => x.Name.Equals(item.SubCategory.Name)).Select(x => x.ID).First();

                if (subcategoryId == 0)
                {
                    _importErrorLogger.LogError(row, $"Subcategory {item.SubCategory.Name} does not exist");
                    continue;
                }

                if (IsValidItem(skuCodes, item, row))
                {
                    item.Attributes = await GetValidAttributes(item);
                    var newItem = await CreateItem(item, subcategoryId, category.ID);
                    savedItems.Add(newItem.GetItemVM());
                    skuCodes.Add(newItem.SKU);  
                }

                await _itemRepository.SaveChanges();
            }

            return StatusCode((int) HttpStatusCode.OK, new ImportItemsResponse
            {
                items = savedItems.ToArray(),
                errors = _importErrorLogger.errors ?? null
            });
        }

        private async Task<Item> CreateItem(ItemVM item, long subcategoryId, long categoryID)
        {
            return await _itemRepository.Insert(new Item
            {
                SKU = item.SKU,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                SubCategoryID = subcategoryId,
                CategoryID = categoryID,
                Pictures = item.Pictures?.Select(q => new ItemPicture
                {
                    URL = q.URL
                })
                .ToList(),
                Attributes = item.Attributes?.Select(q => new AttributeValue
                {
                    AttributeID = q.AttributeID,
                    Value = q.Value
                })
                .ToList()
            });
        }

        private bool IsValidItem(List<string> skuCodes, ItemVM item, int row)
        {
            if (skuCodes.Contains(item.SKU))
            {
                _importErrorLogger.LogError(row, $"Item with SKU: {item.SKU} already exists");
                return false;
            }

            
            return true;
        }

        private async Task<List<ItemAttributesVM>> GetValidAttributes(ItemVM item)
        {
            List<ItemAttributesVM> validAttributes = new List<ItemAttributesVM>();
            // var attributes = await _attributeRepository.GetAll();

            foreach (ItemAttributesVM atr in item.Attributes)
            {
                var savedAtr = await _attributeRepository.FindByName(atr.Name);
                if (savedAtr != null)
                {
                    validAttributes.Add(new ItemAttributesVM
                    {
                        Name = atr.Name,
                        Value = atr.Value,
                        AttributeID = savedAtr.ID
                    });
                }
            }

            return validAttributes;
        }
        
        private string GenerateFileName()
        {
            if (_configuration["ExportFile"] == "CSV")
            {
                return DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss") + "_ItemsExport.csv";
            }
            return DateTime.UtcNow.ToString("yyyy-MM-dd hh-mm-ss") + "_ItemsExport.xlsx";
        }

        private async Task<IActionResult> PerformExport(IEnumerable<ItemVM> items)
        {
            var fileName = GenerateFileName();
            var exportFileDirectory = !string.IsNullOrEmpty(_configuration["ExportedFilesDirectory"]) ? Path.Combine(_configuration["ExportedFilesDirectory"], fileName) : Path.Combine(_hostingEnvironment.ContentRootPath, "ExportedFiles", fileName);
            await _exportService.Export(items, exportFileDirectory);
           

            var obj = new
            {
                urlToFile = $"admin/Items/export/file/{fileName}"
            };
            
            return Ok(obj);
        }

        private List<string> BuildPictureUrlList(List<string> pictureUrls, List<IFormFile> pictureFiles)
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

        private async Task CreateNewAttributes(List<AdminAttributeVM> attributes)
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
