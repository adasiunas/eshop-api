using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;
using eshopAPI.DataAccess;
using eshopAPI.Models;
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

        public CheckoutController(IPaymentService paymentService, ICartRepository cartRepository, IOrderRepository orderRepository, UserManager<ShopUser> userManager, IEmailSender emailSender)
        {
            _paymentService = paymentService;
            _cartRepository = cartRepository;
            _orderRepository = orderRepository;
            _userManager = userManager;
            _emailSender = emailSender;
        }
        
        [HttpPost]
        [IgnoreAntiforgeryToken]
        public async Task<IActionResult> Checkout([FromBody] CheckoutRequest request)
        {
            request.Number = request.Number.Replace(" ", string.Empty);
            Cart cart = await _cartRepository.FindByUser(User.Identity.Name);
            if (cart == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Cart not found."));
            }

            request.Amount= Convert.ToInt32(cart.Items.Sum(i => i.Item.Price * i.Count));
            var user = await _userManager.FindByNameAsync(User.Identity.Name);
            var order = new Order
            {
                User = user,
                Items = cart.Items.Select(i => new OrderItem()
                {
                    Count = i.Count,
                    Price = i.Item.Price,
                    Item = i.Item,
                    ItemID = i.ItemID
                }).ToList(),
                CreateDate = DateTime.UtcNow,
                DeliveryAddress = JsonConvert.SerializeObject(request.Address),
                Status = OrderStatus.Accepted
            };

            order = await _orderRepository.Insert(order);
            
            var paymentResponse = await _paymentService.ProcessPaymentAsync(request);

            if (paymentResponse.IsSuccessfullResponse)
            {
                await _orderRepository.SaveChanges();
                await _emailSender.SendOrderCreationEmailAsync(user.Email, order.ID.ToString());
                return StatusCode((int)HttpStatusCode.OK, "Email sent");
            }
            
            return StatusCode(paymentResponse.ResponseCode, paymentResponse);
        }
    }
}