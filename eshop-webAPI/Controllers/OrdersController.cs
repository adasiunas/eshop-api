using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Controllers
{
    [Produces("application/json")]
    [Route("api/orders")]
    [Authorize]
    [AutoValidateAntiforgeryToken]
    public class OrdersController : ODataController
    {
        private readonly ILogger<ItemsController> _logger;
        private IOrderRepository _orderRepository;

        public OrdersController(
            ILogger<ItemsController> logger, 
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<OrderVM>> Get()
        {
            return await _orderRepository.GetAllOrdersAsQueryable(User.Identity.Name);
        }
    }
}
