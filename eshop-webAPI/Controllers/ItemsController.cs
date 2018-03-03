using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Items")]
    public class ItemsController : Controller
    {
        // GET: api/Items
        [HttpGet]
        public JsonResult Get()
        {
            return new JsonResult(new []{
                new
                {
                    name = "test",
                    img = "https://img.buzzfeed.com/buzzfeed-static/static/2016-08/17/18/campaign_images/buzzfeed-prod-fastlane02/19-poop-facts-that-will-make-you-say-shit-2-22195-1471473131-1_dblbig.jpg"
                },
                new
                {
                    name = "test",
                    img = "https://img.buzzfeed.com/buzzfeed-static/static/2016-08/17/18/campaign_images/buzzfeed-prod-fastlane02/19-poop-facts-that-will-make-you-say-shit-2-22195-1471473131-1_dblbig.jpg"
                },
                new
                {
                    name = "test",
                    img = "https://img.buzzfeed.com/buzzfeed-static/static/2016-08/17/18/campaign_images/buzzfeed-prod-fastlane02/19-poop-facts-that-will-make-you-say-shit-2-22195-1471473131-1_dblbig.jpg"
                },
                new
                {
                    name = "test",
                    img = "https://img.buzzfeed.com/buzzfeed-static/static/2016-08/17/18/campaign_images/buzzfeed-prod-fastlane02/19-poop-facts-that-will-make-you-say-shit-2-22195-1471473131-1_dblbig.jpg"
                }
                });
        }

        // GET: api/Items/5
        [HttpGet("{id}", Name = "Get")]
        public string Get(int id)
        {
            return "value";
        }
        
        // POST: api/Items
        [HttpPost]
        public void Post([FromBody]string value)
        {
        }
        
        // PUT: api/Items/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody]string value)
        {
        }
        
        // DELETE: api/ApiWithActions/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
