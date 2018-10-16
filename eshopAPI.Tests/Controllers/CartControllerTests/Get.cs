using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Services;
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
    public class Get
    {
        CartController _controller;
        private readonly ITestOutputHelper _output;

        public Get(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void ConfirmEmail_Test(bool validUser)
        {
            // Setup
            Cart cart = null;
            if (validUser)
                cart = new Cart { ID = 1, Items = new List<CartItem>() };

            Mock<ICartRepository> cartRepoMock = new Mock<ICartRepository>();
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<ILogger<CartController>> loggerMock = new Mock<ILogger<CartController>>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();

            cartRepoMock.Setup(o => o.FindByUser(It.IsAny<string>()))
                .ReturnsAsync(cart);

            discountRepoMock.Setup(o => o.GetDiscounts())
                .ReturnsAsync(new List<Discount>());

            discountServiceMock.Setup(o => o.CalculateDiscountsForItems(It.IsAny<ICollection<CartItemVM>>(), It.IsAny<List<Discount>>()))
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
            var result = await _controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            if (!validUser)
            {
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            var cartVm = Assert.IsType<CartVM>(objectResult.Value);
            discountServiceMock.Verify(o => o.CalculateDiscountsForItems(It.IsAny<ICollection<CartItemVM>>(), It.IsAny<List<Discount>>()));
        }
    }
}
