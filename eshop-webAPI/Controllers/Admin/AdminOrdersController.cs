using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Models.ViewModels.Admin;
using eshopAPI.Requests.Order;
using eshopAPI.Utils;
using Microsoft.AspNet.OData;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace eshopAPI.Controllers.Admin
{
    [Authorize(Roles = "Admin")]
    [Route("api/admin/orders")]
    public class AdminOrdersController : ODataController
    {
        private ILogger<AdminOrdersController> _logger;
        private IOrderRepository _orderRepository;

        public AdminOrdersController(
            ILogger<AdminOrdersController> logger,
            IOrderRepository orderRepository)
        {
            _logger = logger;
            _orderRepository = orderRepository;
        }

        [EnableQuery]
        [HttpGet]
        public async Task<IQueryable<AdminOrderVM>> Get()
        {
            return await _orderRepository.GetAllAdminOrdersAsQueryable();
        }

        [HttpGet("details")]
        public async Task<IActionResult> GetSingleOrderDetails([FromQuery] long id)
        {
            Order order = await _orderRepository.FindByID(id);

            if(order == null)
            {
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Order was not found"));
            }
            var orderVM = new AdminOrderVM
            {
                ID = order.ID,
                OrderNumber = order.OrderNumber.ToString(),
                DeliveryAddress = order.DeliveryAddress,
                CreateDate = order.CreateDate.ToShortDateString(),
                Status = order.Status.GetDescription(),
                UserEmail = order.User.Email,
                Items = order.Items.Select(i => new OrderItemVM
                {
                    ID = i.ID,
                    Count = i.Count,
                    SKU = i.Item.SKU,
                    ItemID = i.ItemID,
                    Name = i.Item.Name,
                    Price = i.Price
                }),
                TotalPrice = order.Items.Sum(i => i.Price)
            };
            return StatusCode((int)HttpStatusCode.OK, orderVM);
        }

        [HttpPost("changestatus")]
        [Transaction]
        public async Task<IActionResult> ChangeStatus([FromBody] ChangeOrderStatusRequest request)
        {
            _logger.LogInformation($"Changing order staus with ID: {request.ID} to status: {request.Status}");

            Order order = await _orderRepository.FindByID(request.ID);

            if(order == null)
            {
                _logger.LogInformation($"No order found with id {request.ID}");
                return StatusCode((int)HttpStatusCode.NotFound,
                    new ErrorResponse(ErrorReasons.NotFound, "Order was not found"));
            }

            try
            {
                OrderStatus status = (OrderStatus) Enum.Parse(typeof(OrderStatus), request.Status);
                order.Status = status;
            }
            catch (ArgumentException)
            {
                _logger.LogInformation($"Failed to match status {request.Status} to enum");
                return StatusCode((int)HttpStatusCode.BadRequest,
                    new ErrorResponse(ErrorReasons.BadRequest, "Enum with such status name was not found"));
            }

            return StatusCode((int)HttpStatusCode.OK, order);
        }
    }
}
