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
using Microsoft.AspNetCore.Hosting;
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

namespace eshopAPI.Tests.Controllers.ItemsControllerTests
{
    public class Get
    {
        ItemsController _controller;

        public Get()
        {
        }

        [Fact]
        public async void Success()
        {
            // Setup
            List<Discount> discounts = new List<Discount> { };
            List<ItemVM> items = new List<ItemVM> {
                new ItemBuilder().Random().Build().GetItemVM(),
                new ItemBuilder().Random().Build().GetItemVM(),
                new ItemBuilder().Random().Build().GetItemVM()
            };

            Mock<ILogger<ItemsController>> loggerMock = new Mock<ILogger<ItemsController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
            Mock<IImageCloudService> imageServiceMock = new Mock<IImageCloudService>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IPaymentService> paymentServiceMock = new Mock<IPaymentService>();
            Mock<ICategoryRepository> categoryRepoMock = new Mock<ICategoryRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();
            Mock<IHostingEnvironment> hostingEnvMock = new Mock<IHostingEnvironment>();

            discountRepoMock.Setup(o => o.GetDiscounts())
                .ReturnsAsync(discounts);

            itemRepoMock.Setup(o => o.GetAllItemsForFirstPage())
                .ReturnsAsync(items);

            _controller = new ItemsController(
                loggerMock.Object,
                configurationMock.Object,
                imageServiceMock.Object,
                itemRepoMock.Object,
                discountRepoMock.Object,
                paymentServiceMock.Object,
                categoryRepoMock.Object,
                discountServiceMock.Object,
                hostingEnvMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Get();

            // Assert
            Assert.Equal(3, result.Count());
        }
    }
}
