using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Requests.Cart;
using eshopAPI.Services;
using eshopAPI.Tests.Builders;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Security.Claims;
using System.Security.Principal;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Xunit;
using Xunit.Abstractions;

namespace eshopAPI.Tests.Controllers.CartControllerTests
{
    public class GetCartItemsCount
    {
        CartController _controller;
        private readonly ITestOutputHelper _output;

        public GetCartItemsCount(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void GetCartItemsCount_Test(bool userHasCart)
        {
            // Setup
            Cart cart = new Cart
            {
                ID = 1,
                Items = new List<CartItem> {
                    new CartItemBuilder().Random().SetCount(1).Build(),
                    new CartItemBuilder().Random().SetCount(2).Build()
                }
            };
            
            Mock<ICartRepository> cartRepoMock = new Mock<ICartRepository>();
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<ILogger<CartController>> loggerMock = new Mock<ILogger<CartController>>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();

            cartRepoMock.Setup(o => o.FindByUser(It.IsAny<string>()))
                .ReturnsAsync(userHasCart ? cart : null);

            _controller = new CartController(cartRepoMock.Object,
                userRepoMock.Object,
                itemRepoMock.Object,
                loggerMock.Object,
                discountRepoMock.Object,
                discountServiceMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.GetCartItemsCount();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            if (!userHasCart)
            {
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }

            var count = Assert.IsType<int>(objectResult.Value);
            Assert.Equal(3, count);
        }
    }
}
