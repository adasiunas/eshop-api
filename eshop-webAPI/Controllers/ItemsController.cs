using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/items")]
    [AutoValidateAntiforgeryToken]
    public class ItemsController : ODataController
    {
        private IItemRepository _itemRepository;
        public ItemsController(IItemRepository itemRepository)
        {
            _itemRepository = itemRepository;
        }

        [EnableQuery]
        [HttpGet]
        [AllowAnonymous]
        public IQueryable<ItemVM> Get()
        {
            return _itemRepository.GetAllItemsForFirstPageAsQueryable();
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
