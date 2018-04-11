using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.Services;
using log4net.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/items")]
    [AutoValidateAntiforgeryToken]
    public class ItemsController : Controller
    {
        private readonly ILogger<ItemsController> _logger;
        private readonly IConfiguration _configuration;
        private readonly IImageCloudService _imageCloudService;
        public ItemsController(ILogger<ItemsController> logger, IConfiguration configuration, IImageCloudService imageCloudService)
        {
            _configuration = configuration;
            _logger = logger;
            _imageCloudService = imageCloudService;
        }
        // GET: api/Items
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(new[]{
                new
                {
                    SKU = "12134123",
                    Name = "Men\'s jacket Rahfa",
                    Image = "https://style24.lt/image/cache/data/products/82/vyriskas-megztinis-rahfa-39052-750x750.jpg",
                    Price = 37.95,
                    Attributes = new[]{
                        new {Name = "Color", Value = "blue"},
                        new {Name= "Style", Value= "casual"}
                    }
                },
                new
                {
                    SKU = "12144123",
                    Name = "Mountainskin Thicken Fleece Winter Jackets Men\'s Coats 5XL Cotton Fur Collar Men\'s Jackets Military Casual Male Outerwear SA351",
                    Image = "https://ae01.alicdn.com/kf/HTB1Tzy9SpXXXXaKXpXXq6xXFXXXX/Mountainskin-Thicken-Fleece-Winter-Jackets-Men-s-Coats-5XL-Cotton-Fur-Collar-Men-s-Jackets-Military.jpg_640x640.jpg",
                    Price = 31.89,
                    Attributes = new[]{
                        new {Name = "Color", Value = "brown"},
                        new {Name= "Style", Value= "casual"}
                    }
                },
                new
                {
                    SKU = "12144122223",
                    Name = "New Version Jumper EZbook 3 Pro Dual Band AC Wifi laptop with M.2 SATA SSD Slot Apollo Lake N3450 13.3' IPS 6GB DDR3 ultrabook",
                    Image = "https://ae01.alicdn.com/kf/HTB1WgwNQVXXXXXrXFXXq6xXFXXXP/New-Version-Jumper-EZbook-3-Pro-Dual-Band-AC-Wifi-laptop-with-M-2-SATA-SSD.jpg_640x640.jpg",
                    Price = 322.83,
                    Attributes = new[]{
                        new {Name = "Color", Value = "white"}
                    }
                },
                new
                {
                    SKU = "1212223",
                    Name = "2pcs/Sets Syllable X2 TWS Bluetooth Speaker Outdoor Wireless Speaker Waterproof IPX5 Compatible Stereo Soundbar with mic",
                    Image = "https://ae01.alicdn.com/kf/HTB17.Sdb7CWBuNjy0Faq6xUlXXaH/2pcs-Sets-Syllable-X2-TWS-Bluetooth-Speaker-Outdoor-Wireless-Speaker-Waterproof-IPX5-Compatible-Stereo-Soundbar-with.jpg_640x640.jpg",
                    Price = 70.00,
                    Attributes = new[]{
                        new {Name= "", Value= "" }
                    }
                },
                new
                {
                    SKU = "5551212223",
                    Name = "2018 New Arrival Famous Brand Business Men Briefcase Bag PU Leather Laptop Bag Briefcase Male PU Leather Shoulder bags",
                    Image = "https://ae01.alicdn.com/kf/HTB1kQvIfDnI8KJjSszbq6z4KFXar/2018-New-Arrival-Famous-Brand-Business-Men-Briefcase-Bag-PU-Leather-Laptop-Bag-Briefcase-Male-PU.jpg_640x640.jpg",
                    Price = 35.00,
                    Attributes = new[]{
                        new {Name= "", Value= "" }
                    }
                },
                new
                {
                    SKU = "5551255512223",
                    Name = "Tenda AC7 wireless wifi Routers 11AC 2.4Ghz/5.0Ghz Wi-fi Repeater 1*WAN+3*LAN 5*6dbi high gain Antennas Smart APP Manage",
                    Image = "https://ae01.alicdn.com/kf/HTB1wJd5cBLN8KJjSZFpq6zZaVXaL/Tenda-AC6-Dual-Band-1200Mbps-Wifi-Router-WI-FI-Repeater-Wireless-WIFI-Router-11AC-2-4G.jpg_640x640.jpg",
                    Price = 99.99,
                    Attributes = new[]{
                        new {Name= "Color", Value= "black"}
                    }
                },
                new
                {
                    SKU = "55512223",
                    Name = "Tardoo Fox Shape Genuine 925 Sterling Silver Stud Earrings for Women Lovely & Classic Style Brand Fine Jewelry",
                    Image = "https://ae01.alicdn.com/kf/HTB1CiCZbRTH8KJjy0Fiq6ARsXXaZ/Tardoo-Fox-Shape-Genuine-925-Sterling-Silver-Stud-Earrings-for-Women-Lovely-Classic-Style-Brand-Fine.jpg_640x640.jpg",
                    Price = 30.55,
                    Attributes = new[]{
                        new {Name = "Material", Value = "silver"}
                    }
                },
                new
                {
                    SKU = "5435512223",
                    Name = "Classic Special Design 8-9mm Natural Pearl Pendant Necklace with 45cm Silver Chain 4 colors Wholesale Casual Jewelry",
                    Image = "https://ae01.alicdn.com/kf/HTB1oPrAPXXXXXbsapXXq6xXFXXXC/Classic-Special-Design-8-9mm-Natural-Pearl-Pendant-Necklace-with-45cm-Silver-Chain-4-colors-Wholesale.jpg_640x640.jpg",
                    Price = 99.00,
                    Attributes = new[]{
                        new {Name = "Material", Value = "silver"}
                    }
                },
                new
                {
                    SKU = "56999223",
                    Name = "SNH 8-9mm near round A 925silver 100% real white freshwater pearl necklace for woman three rows pearl necklace",
                    Image = "https://ae01.alicdn.com/kf/HTB1v3YcKFXXXXcDXFXXq6xXFXXXX/SNH-8-9mm-near-round-A-925silver-100-real-white-freshwater-pearl-necklace-for-woman-three.jpg_640x640.jpg",
                    Price = 42.00,
                    Attributes = new[]{
                        new {Name = "Material", Value = "silver"}
                    }
                },
                new
                {
                    SKU = "999223",
                    Name = "BailiLaoRen Business Briefcase Leather Man 14-15 Laptop Handbags Large-Capacity Travel Men's Messenger Crossbody Bag P083",
                    Image = "https://ae01.alicdn.com/kf/HTB1zyuWQpXXXXcVXpXXq6xXFXXXt/BailiLaoRen-Business-Briefcase-Leather-Man-14-15-Laptop-Handbags-Large-Capacity-Travel-Men-s-Messenger-Crossbody.jpg_640x640.jpg",
                    Price = 302.00,
                    Attributes = new[]{
                        new {Name = "Color", Value = "dark brown"},
                        new {Name = "Material", Value = "leather"}
                    }
                },
                new
                {
                    SKU = "34999223",
                    Name = "Fashion Nylon Men's Briefcase tote 15.6laptop bag Business Case men Handbag male shoulder bag",
                    Image = "https://ae01.alicdn.com/kf/HTB1Z5KXPFXXXXadXXXXq6xXFXXXn/Fashion-Nylon-Men-s-Briefcase-tote-15-6-laptop-bag-Business-Case-men-Handbag-male-shoulder.jpg_640x640.jpg",
                    Price = 13.00,
                    Attributes = new[]{
                        new {Name = "Color", Value = "brown"}
                    }
                }
                });
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
