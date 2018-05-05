using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Cart;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/Cart")]
    [AutoValidateAntiforgeryToken]
    [Authorize]
    public class CartController : Controller
    {
        private readonly ICartRepository _cartRepository;
        private readonly IShopUserRepository _userRepository;
        private readonly IItemRepository _itemRepository;
        private readonly ILogger<CartController> _logger;

        public CartController(ICartRepository cartRepository, IShopUserRepository userRepository, IItemRepository itemRepository, ILogger<CartController> logger)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            _logger = logger;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<Cart> Get()
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            return cart;
        }
        
        // POST: api/Cart
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CartRequest request)
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            ShopUser user = await _userRepository.GetUserWithEmail(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogInformation("Creating new cart for user - " + User.Identity.Name);
                cart = new Cart { User = user };
                _cartRepository.Insert(cart);
            }

            cart.Items = new List<CartItem>();
            foreach(var itemRequest in request.Items)
            {
                Item item = await _itemRepository.FindByID(itemRequest.ItemID);
                if (item == null)
                {
                    _logger.LogError("Item with ID - " + itemRequest.ItemID + " was not found.");
                    return BadRequest("Attempting to add non-existing item to cart.");
                }

                CartItem cartItem = new CartItem { Item = item, Count = itemRequest.Count };
                cart.Items.Add(cartItem);
            }
            await _cartRepository.SaveChanges();

            _logger.LogInformation("New cart was created.");
            return Ok();
        }
        
        // PUT: api/Cart
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]CartItemRequest itemRequest)
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogInformation("Creating new cart for user - " + User.Identity.Name);
                ShopUser user = await _userRepository.GetUserWithEmail(User.Identity.Name);
                cart = new Cart { User = user, Items = new List<CartItem>() };
                _cartRepository.Insert(cart);
            }

            Item item = await _itemRepository.FindByID(itemRequest.ItemID);
            if (item == null)
            {
                _logger.LogError("Item with ID - " + itemRequest.ItemID + " was not found.");
                return BadRequest("Attempting to add non-existing item to cart.");
            }
            CartItem existingItem = cart.Items.Where(i => i.ItemID == item.ID).FirstOrDefault();
            if (existingItem == null)
            {
                _logger.LogInformation("Adding new item to cart.");
                cart.Items.Add(new CartItem { Item = item, Count = itemRequest.Count });
            }
            else
            {
                _logger.LogInformation("Item is already in cart, changing count from " + existingItem.Count + " to " + existingItem.Count + itemRequest.Count);
                existingItem.Count += itemRequest.Count;
            }
            await _cartRepository.SaveChanges();
            return Ok();
        }
        
        // DELETE: api/deletecartitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem([FromQuery] int id)
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogError("Trying to remove item but cart does not exist");
                return BadRequest("Cart does not exist");
            }
            CartItem itemToRemove = cart.Items.Where(c => c.ID == id).FirstOrDefault();
            cart.Items.Remove(itemToRemove);
            await _cartRepository.SaveChanges();

            return Ok();
        }
    }
}
