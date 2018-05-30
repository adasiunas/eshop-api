using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Requests.Cart;
using eshopAPI.Services;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
        private readonly IDiscountRepository _discountRepository;
        private readonly IDiscountService _discountService;

        public CartController(
            ICartRepository cartRepository,
            IShopUserRepository userRepository,
            IItemRepository itemRepository,
            ILogger<CartController> logger,
            IDiscountRepository discountRepository,
            IDiscountService discountService)
        {
            _cartRepository = cartRepository;
            _userRepository = userRepository;
            _itemRepository = itemRepository;
            _logger = logger;
            _discountRepository = discountRepository;
            _discountService = discountService;
        }

        // GET: api/Cart
        [HttpGet]
        public async Task<IActionResult> Get()
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Cart not found."));
            }
            var cartVm = cart.GetCartVM();
            var discounts = await _discountRepository.GetDiscounts();
            _discountService.CalculateDiscountsForItems(cartVm.Items, discounts);
            return StatusCode((int)HttpStatusCode.OK, cart.GetCartVM());
        }
        
        // POST: api/Cart
        [HttpPost]
        [Transaction]
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

            _logger.LogInformation(notFoundCount + " items was not found and could not be added to cart");
            return StatusCode((int) HttpStatusCode.OK, notFoundCount);
        }
        
        // PUT: api/Cart
        [HttpPut]
        [Transaction]
        public async Task<IActionResult> Put([FromBody]CartItemRequest itemRequest)
        {
            Cart cart = await GetUserCart();

            bool itemAdded = await AddItemToCart(cart, itemRequest);
            if (itemAdded)
            {
                return StatusCode((int) HttpStatusCode.NoContent);
            }

            return StatusCode((int)HttpStatusCode.NotFound,
                new ErrorResponse(ErrorReasons.NotFound, "Item you want to add to cart does not exist."));
        }

        // PUT: api/Cart/updatecartitems
        [HttpPut("updatecartitems")]
        [Transaction]
        public async Task<IActionResult> UpdateCartItems([FromBody]CartRequest cartRequest)
        {
            Cart cart = await GetUserCart();
            if (cart == null)
                return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse(ErrorReasons.NotFound, "Cart not found."));

            foreach (var item in cartRequest.Items)
            {
                CartItem cartItem = cart.Items.FirstOrDefault(i => i.ID == item.ItemID);
                if(cartItem == null)
                    return StatusCode((int)HttpStatusCode.NotFound, new ErrorResponse(ErrorReasons.NotFound, "Item cannot be updateded because it is not found."));

                cartItem.Count = item.Count;
            }
            return StatusCode((int)HttpStatusCode.NoContent);
        }

        // DELETE: api/Cart/deletecartitem/{id}
        [HttpDelete("deletecartitem/{id}")]
        [Transaction]
        public async Task<IActionResult> DeleteCartItem(int id)
        {
            Cart cart = await _cartRepository.FindByUserWithoutItemsData(User.Identity.Name);
            if (cart == null)
            {
                _logger.LogError("Trying to remove item but cart does not exist");
                return StatusCode((int) HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Cart does not exist"));
            }
            CartItem itemToRemove = cart.Items.Where(c => c.ID == id).FirstOrDefault();
            await _cartRepository.RemoveCartItem(itemToRemove);

            return StatusCode((int)HttpStatusCode.NoContent);
        }

        private async Task<Cart> GetUserCart()
        {
            Cart cart = await _cartRepository.FindByUserWithoutItemsData(User.Identity.Name);
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
        // GET: api/Cart/itemsCount
        [HttpGet("itemsCount")]
        public async Task<IActionResult> GetCartItemsCount()
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Cart not found."));
            }
            int count = cart.Items.Select(c => c.Count).Sum();
            return StatusCode((int)HttpStatusCode.OK, count);
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
