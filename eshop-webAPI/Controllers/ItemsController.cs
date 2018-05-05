using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Services;
using log4net.Core;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Response;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

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
        private IItemRepository _itemRepository;
        private readonly IPaymentService _paymentService;
        public ItemsController(ILogger<ItemsController> logger, IConfiguration configuration, IImageCloudService imageCloudService, IItemRepository itemRepository, IPaymentService paymentService)
        {
            _configuration = configuration;
            _logger = logger;
            _imageCloudService = imageCloudService;
            _itemRepository = itemRepository;
            _paymentService = paymentService;
        }
        
        // GET: api/Items
        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IQueryable<ItemVM> Get()
        {
            return _itemRepository.GetAllItemsForFirstPageAsQueryable();
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
            return Ok(result.ToArray());
        }

        [HttpPost("testPaymentService")]
        [AllowAnonymous]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> TestPayment([FromBody] PaymentRequest request)
        {
            var paymentResponse = await _paymentService.ProcessPaymentAsync(request);
            
            return StatusCode(paymentResponse.ResponseCode, paymentResponse);
        }

        [HttpGet("itemdetails/{id}")]
        [AllowAnonymous]
        public async Task<IActionResult> ItemDetails([FromRoute] long id)
        {
            Item item = await _itemRepository.FindByID(id);
            if (item == null)
                return BadRequest("Item not found");

            return Ok(item.GetItemVM());
        }
    }
}
