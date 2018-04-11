using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Services;
using log4net.Core;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/items")]
    [AutoValidateAntiforgeryToken]
    public class ItemsController : ODataController
    {
<<<<<<< eshop-webAPI/Controllers/ItemsController.cs
        private readonly ILogger<ItemsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IImageCloudService _imageCloudService;
        private IItemRepository _itemRepository;
        public ItemsController(ILogger<ItemsController> logger, IConfiguration configuration, IImageCloudService imageCloudService, IItemRepository itemRepository)
        {
            _configuration = configuration;
            _logger = logger;
            _imageCloudService = imageCloudService;
            _itemRepository = itemRepository;
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
    }
}
