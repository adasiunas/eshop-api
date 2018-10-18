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
    public class DeleteCartItem
    {
        CartController _controller;
        private readonly ITestOutputHelper _output;

        public DeleteCartItem(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async void DeleteCartItem_Test(bool userHasCart, bool itemExistsInCart)
        {
            // Setup
            int requestId = 0;
            Cart cart = new Cart
            {
                ID = 1,
                Items = new List<CartItem> {
                    new CartItemBuilder().Random().Build(),
                    new CartItemBuilder().Random().Build()
                }
            };

            if (itemExistsInCart)
                requestId = (int)cart.Items.First().ID;
            
            Mock<ICartRepository> cartRepoMock = new Mock<ICartRepository>();
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<ILogger<CartController>> loggerMock = new Mock<ILogger<CartController>>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();

            cartRepoMock.Setup(o => o.FindByUserWithoutItemsData(It.IsAny<string>()))
                .ReturnsAsync(userHasCart ? cart : null);

            cartRepoMock.Setup(o => o.RemoveCartItem(It.IsAny<CartItem>()))
                .Returns(Task.FromResult(0))
                .Verifiable();
            
            _controller = new CartController(cartRepoMock.Object,
                userRepoMock.Object,
                itemRepoMock.Object,
                loggerMock.Object,
                discountRepoMock.Object,
                discountServiceMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.DeleteCartItem(requestId);

            // Assert
            if (!userHasCart)
            {
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }

            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
            
        }
    }
}
