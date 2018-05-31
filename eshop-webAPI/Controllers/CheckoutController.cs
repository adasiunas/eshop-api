using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Requests;
using eshopAPI.Services;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/checkout")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class CheckoutController : Controller
    {
        private readonly IPaymentService _paymentService;
        private readonly ICartRepository _cartRepository;
        private readonly IOrderRepository _orderRepository;
        private readonly UserManager<ShopUser> _userManager;
        private readonly IEmailSender _emailSender;
        private readonly IDiscountService _discountService;
        private readonly IDiscountRepository _discountRepository;

        public CheckoutController(
            IPaymentService paymentService,
            ICartRepository cartRepository,
            IOrderRepository orderRepository,
            IDiscountService discountService,
            IDiscountRepository discountRepository,
            UserManager<ShopUser> userManager,
            IEmailSender emailSender)
        {
            _paymentService = paymentService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _discountService = discountService;
            _discountRepository = discountRepository;
            _emailSender = emailSender;
        }
        
        [HttpPost]
        [Transaction]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Cart not found."));
            }
            var discounts = await _discountRepository.GetDiscounts();
            var cartVm = cart.GetCartVM();
            _discountService.CalculateDiscountsForItems(cartVm.Items, discounts);

            request.Amount= Convert.ToInt64(cartVm.Items.Sum(i => (i.Discount == 0 ? i.Price : i.Discount) * i.Count * 100));
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            
            var order = new Order
            {
                User = user,
                Items = cartVm.Items.Select(i => new OrderItem()
                {
                    Count = i.Count,
                    Price = i.Discount == 0 ? i.Price : i.Discount,
                    ItemID = i.ItemID
                }).ToList(),
                DeliveryAddress = JsonConvert.SerializeObject(request.Address),
                Status = OrderStatus.Accepted
            };

            order = await _orderRepository.Insert(order);
            
            request.Number = request.Number.Replace(" ", string.Empty);
            var paymentResponse = await _paymentService.ProcessPaymentAsync(request);

            if (paymentResponse.IsSuccessfullResponse)
            {
                await _orderRepository.SaveChanges();
                await _emailSender.SendOrderCreationEmailAsync(user.Email, order.OrderNumber.ToString());
                await _cartRepository.ClearCart(cart);
                return StatusCode((int)HttpStatusCode.OK, "Email sent");
            }
            
            return StatusCode(paymentResponse.ResponseCode, paymentResponse);
        }
    }
}