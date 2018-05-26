using System;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
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
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IConfiguration _configuration;
        private readonly IExportService _exportService;

        public AdminItemsController(
            ILogger<ItemsController> logger,
            IItemRepository itemRepository,
            ICategoryRepository categoryRepository, IHostingEnvironment hostingEnvironment, IConfiguration configuration, IExportService exportService)
        {
            _logger = logger;
            _itemRepository = itemRepository;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _exportService = exportService;
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
        
        [HttpGet("export/{categoryId}")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Export(int? categoryId)
        {
            var items = await _itemRepository.GetAllItemsForFirstPageAsQueryable();
            if (categoryId != null)
            {
                items = items.Where(i => i.ItemCategory.ID == categoryId);
                if (!items.Any())
                {
                    return StatusCode((int)HttpStatusCode.BadRequest, "No items for this category was found.");
                }
            }
            var fileName = GenerateFileName();
            var exportFileDirectory = !string.IsNullOrEmpty(_configuration["ExportedFilesDirectory"]) ? Path.Combine(_configuration["ExportedFilesDirectory"], fileName) : Path.Combine(_hostingEnvironment.ContentRootPath, "ExportedFiles", fileName);
            await _exportService.Export(items.AsEnumerable(), exportFileDirectory);
            byte[] bytes;
            using (var fileStream = new FileStream(exportFileDirectory, FileMode.Open, FileAccess.Read))
            {
                bytes = new byte[fileStream.Length];
                int numBytesToRead = (int) fileStream.Length;
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

        [HttpGet("exportAll")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> ExportAll()
        {
            return await Export(categoryId: null);
        }
        
        public string GenerateFileName()
        {
            return DateTime.UtcNow.ToString("yyyy-MM-dd hh:mm:ss") + "_ItemsExport.xlsx";
        }

        

    }
}
