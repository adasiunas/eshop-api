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
        private readonly IPaymentService _paymentService;
        private readonly ICategoryRepository _categoryRepository;

        public ItemsController(
            ILogger<ItemsController> logger,
            IConfiguration configuration,
            IImageCloudService imageCloudService,
            IItemRepository itemRepository,
            IPaymentService paymentService,
            ICategoryRepository categoryRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _imageCloudService = imageCloudService;
            _itemRepository = itemRepository;
            _paymentService = paymentService;
            _categoryRepository = categoryRepository;
        }
        
        // GET: api/Items
        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public async Task<IQueryable<ItemVM>> Get()
        {
            return await _itemRepository.GetAllItemsForFirstPageAsQueryable();
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

        [HttpPost("testPaymentService")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TestPayment([FromBody] PaymentRequest request)
        {
            var paymentResponse = await _paymentService.ProcessPaymentAsync(request);
            
            return StatusCode(paymentResponse.ResponseCode, paymentResponse);
        }
    }
}
