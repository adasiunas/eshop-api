using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Services;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using System.Net;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Hosting;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/items")]
    [AutoValidateAntiforgeryToken]
    public class ItemsController : ODataController
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IImageCloudService _imageCloudService;
        private readonly IItemRepository _itemRepository;
        private readonly IDiscountRepository _discountRepository;
        private readonly IPaymentService _paymentService;
        private readonly ICategoryRepository _categoryRepository;
        private readonly IHostingEnvironment _hostingEnvironment;

        public ItemsController(
            ILogger<ItemsController> logger,
            IConfiguration configuration,
            IImageCloudService imageCloudService,
            IItemRepository itemRepository,
            IDiscountRepository discountRepository,
            IPaymentService paymentService,
            ICategoryRepository categoryRepository, IHostingEnvironment hostingEnvironment)
        {
            _configuration = configuration;
            _logger = logger;
            _imageCloudService = imageCloudService;
            _itemRepository = itemRepository;
            _discountRepository = discountRepository;
            _paymentService = paymentService;
            _categoryRepository = categoryRepository;
            _hostingEnvironment = hostingEnvironment;
        }
        
        // GET: api/Items
        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IQueryable<ItemVM>> Get()
        {
            var discounts = (await _discountRepository.GetAllValidDiscounts()).ToList();
            var items = _itemRepository.GetAllItemsForFirstPageAsQueryable(discounts);

            return await items;
        }

        [HttpPost]
        public async Task<IActionResult> UploadImages([FromForm]IEnumerable<IFormFile> images)
        {
            var listOfImageStreams = new List<Stream>();
            foreach (var image in images)
            {
                var imgStream = new MemoryStream();
                await image.CopyToAsync(imgStream);
                listOfImageStreams.Add(imgStream);
            }

            var result = _imageCloudService.UploadImagesFromFiles(listOfImageStreams);
            return StatusCode((int) HttpStatusCode.OK, result.ToArray());
        }

    }
}
