using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eshopAPI.Controllers.Admin
{
    [Route("api/admin/orders")]
    public class OrdersController : Controller
    {
        private IOrderRepository _orderRepository;
        private IShopUserRepository _shopUserRepository;
        public OrdersController(
            IOrderRepository orderRepository,
            IShopUserRepository shopUserRepository)
        {
            _orderRepository = orderRepository;
            _shopUserRepository = shopUserRepository;
        }

        [HttpGet("single")]
        public async Task<IActionResult> GetOrders([FromQuery] string email)
        {
            ShopUserProfile user = await _shopUserRepository.GetUserProfile(email);

            if(user == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, $"User with email ${email} not found"));
            }

            return StatusCode((int) HttpStatusCode.OK, (await _orderRepository.GetAllOrdersAsQueryable(email)).ToListAsync());
        }
    }
}
