using eshopAPI.DataAccess;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.Models;

namespace eshopAPI.Controllers.Admin
{
    [Route("api/admin/attributes")]
    public class AttributesController : Controller
    {
        private readonly ILogger<AttributesController> _logger;
        private readonly IAttributeRepository _attributeRepository;
        public AttributesController(
            ILogger<AttributesController> logger,
            IAttributeRepository attributeRepository)
        {
            _logger = logger;
            _attributeRepository = attributeRepository;
        }

        [HttpGet("names")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetAttributeNames([FromQuery] string filter = "")
        {
            List<Attribute> attributes = await _attributeRepository.FindAttributeNamesByText(filter);
            return StatusCode((int)HttpStatusCode.OK, attributes);
        }

        [HttpGet("values")]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> GetAttributeValue([FromQuery] int id)
        {
            List<AttributeValue> attributes = await _attributeRepository.FindAttributeValuesById(id);
            return StatusCode((int)HttpStatusCode.OK, attributes);
        }
    }
}
