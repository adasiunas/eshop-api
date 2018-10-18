using eshop_webAPI.Controllers;
using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Requests.Account;
using eshopAPI.Response;
using eshopAPI.Services;
using eshopAPI.Tests.Builders;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Xunit;

namespace eshopAPI.Tests.Controllers.OrdersControllerTests
{
    public class Get
    {
        OrdersController _controller;

        public Get()
        {
        }

        [Fact]
        public async void Success()
        {
            // Setup 
            Mock<ILogger<ItemsController>> loggerMock = new Mock<ILogger<ItemsController>>();
            Mock<IOrderRepository> orderRepoMock = new Mock<IOrderRepository>();

            _controller = new OrdersController(
                loggerMock.Object,
                orderRepoMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Get();

            // Assert
            orderRepoMock.Verify(o => o.GetAllOrdersAsQueryable(It.IsAny<string>()));
        }
    }
}
