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
using eshopAPI.Requests;
using eshopAPI.Response;
using eshopAPI.Utils;
using eshopAPI.Utils.Import;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private ImportErrorLogger _importErrorLogger;
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IExportService _exportService;
        private readonly IImportService _importService;

        public AdminItemsController(
            ILogger<ItemsController> logger,
            IItemRepository itemRepository,
            ICategoryRepository categoryRepository, 
            IHostingEnvironment hostingEnvironment, 
            IConfiguration configuration, 
            IExportService exportService,
            IImportService importService)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _exportService = exportService;
            _importService = importService;
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
                return StatusCode((int)HttpStatusCode.NotFound,
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

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        [HttpGet("export/{categoryId}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Export(int categoryId)
        {
            var items = await _itemRepository.GetAllItemsForFirstPageAsQueryable();
            items = items.Where(i => i.ItemCategory.ID == categoryId);
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items for this category was found."));
            }

            return await PerformExport(items);
        }

        [HttpGet("exportAll")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ExportAll()
        {
            
            var items = await _itemRepository.GetAllItemsForFirstPageAsQueryable();
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items was found."));
            }
            return await PerformExport(items);
        }
        
        
        [HttpGet("export/subcategory/{subcategoryId}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ExportSubcategory(int subcategoryId)
        {
            var items = await _itemRepository.GetAllItemsForFirstPageAsQueryable();
            items = items.Where(i => i.ItemCategory.SubCategory.ID == subcategoryId);
            if (!items.Any())
            {
                return StatusCode((int)HttpStatusCode.BadRequest, new ErrorResponse(ErrorReasons.NotFound, "No items for this subcategory was found."));
            }

            return await PerformExport(items);
        }
        
        private string GenerateFileName()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss") + "_ItemsExport.xlsx";
        }

        private async Task<FileContentResult> PerformExport(IEnumerable<ItemVM> items)
        {
            var fileName = GenerateFileName();
            var exportFileDirectory = !string.IsNullOrEmpty(_configuration["ExportedFilesDirectory"]) ? Path.Combine(_configuration["ExportedFilesDirectory"], fileName) : Path.Combine(_hostingEnvironment.ContentRootPath, "ExportedFiles", fileName);
            await _exportService.Export(items, exportFileDirectory);
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
            return File(bytes, "application/octet-stream", fileName);
        }



        [HttpGet("import")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
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
            await Task.Delay(5000);

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
                // long subcategoryId = 1;

                Category category = await _categoryRepository.FindByName(item.ItemCategory.Name);
                if (category == null)
                {
                    _importErrorLogger.LogError(row, $"Category {item.ItemCategory.Name} does not exist");
                    //return false;
                    continue;
                }

                var subcategories = await _categoryRepository.GetChildrenOfParent(category.ID);
                long subcategoryId = subcategories.Where(x => x.Name.Equals(item.ItemCategory.SubCategory.Name)).Select(x => x.ID).First();

                if (subcategoryId == 0)
                {
                    _importErrorLogger.LogError(row, $"Subcategory {item.ItemCategory.SubCategory.Name} does not exist");
                    //return false;
                    continue;
                }

                // subcategoryId must be returned from validation ir retrieved here

                if (isValidItem(skuCodes, item, row))
                {
                    // TODO: validate attributes

                    var newItem = await CreateItem(item, subcategoryId);

                    if (newItem != null)
                    {
                        savedItems.Add(newItem.GetItemVM());
                        skuCodes.Add(newItem.SKU);
                    }
                }
            }
            
            await _itemRepository.SaveChanges();

            return StatusCode((int) HttpStatusCode.OK, new ImportItemsResponse
            {
                items = savedItems.ToArray(),
                errors = _importErrorLogger.errors ?? null
            });
        }

        private async Task<Item> CreateItem(ItemVM item, long subcategoryId)
        {
            return await _itemRepository.Insert(new Item
            {
                SKU = item.SKU,
                Name = item.Name,
                Description = item.Description,
                Price = item.Price,
                SubCategoryID = subcategoryId,
                Pictures = item.Pictures.Select(q => new ItemPicture
                    {
                        URL = q.URL
                    })
                    .ToList()
            });
        }

        private bool isValidItem(List<string> skuCodes, ItemVM item, int row)
        {
            if (skuCodes.Contains(item.SKU))
            {
                _importErrorLogger.LogError(row, $"Item with SKU: {item.SKU} already exists");
                return false;
            }

            
            return true;
        }
    }
}
