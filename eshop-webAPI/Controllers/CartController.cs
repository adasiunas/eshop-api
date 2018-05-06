using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Cart;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authorization;
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
        public async Task<IActionResult> Get()
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            return StatusCode((int) HttpStatusCode.OK, cart);
        }
        
        // POST: api/Cart
        [HttpPost]
        public async Task<IActionResult> Post([FromBody]CartRequest request)
        {
            Cart cart = await GetUserCart();

            int notFoundCount = 0;
            foreach(var itemRequest in request.Items)
            {
                bool added = await AddItemToCart(cart, itemRequest);
                if (!added)
                    notFoundCount++;
            }
            await _cartRepository.SaveChanges();

            _logger.LogInformation(notFoundCount + " items was not found and could not be added to cart");
            return StatusCode((int) HttpStatusCode.OK, notFoundCount);
        }
        
        // PUT: api/Cart
        [HttpPut]
        public async Task<IActionResult> Put([FromBody]CartItemRequest itemRequest)
        {
            Cart cart = await GetUserCart();

            bool itemAdded = await AddItemToCart(cart, itemRequest);
            if (itemAdded)
            {
                await _cartRepository.SaveChanges();
                return StatusCode((int) HttpStatusCode.NoContent);
            }

            return StatusCode((int)HttpStatusCode.NotFound,
                new ErrorResponse(ErrorReasons.NotFound, "Item you want to add to cart does not exist."));
        }
        
        // DELETE: api/deletecartitem/{id}
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteCartItem([FromQuery] int id)
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogError("Trying to remove item but cart does not exist");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "There are no cart created for this user."));
            }
            CartItem itemToRemove = cart.Items.Where(c => c.ID == id).FirstOrDefault();
            cart.Items.Remove(itemToRemove);
            await _cartRepository.SaveChanges();

            return StatusCode((int) HttpStatusCode.NoContent);
        }

        private async Task<Cart> GetUserCart()
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogInformation("Creating new cart for user - " + User.Identity.Name);
                ShopUser user = await _userRepository.GetUserWithEmail(User.Identity.Name);
                cart = new Cart
                {
                    User = user,
                    Items = new List<CartItem>()
                };
                await _cartRepository.Insert(cart);
            }
            return cart;
        }

        private async Task<bool> AddItemToCart(Cart cart, CartItemRequest itemRequest)
        {
            Item item = await _itemRepository.FindByID(itemRequest.ItemID);
            if (item == null)
            {
                _logger.LogError("Item with ID - " + itemRequest.ItemID + " was not found.");
                return false;
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
            return true;
        }
    }
}
