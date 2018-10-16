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
    public class Put
    {
        CartController _controller;
        private readonly ITestOutputHelper _output;

        public Put(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false, false, false, false)]
        [InlineData(true, true, true, false)]
        [InlineData(true, true, false, false)]
        [InlineData(true, true, false, true)]
        public async void ConfirmEmail_Test(bool userHasCart, bool itemFound, bool itemDeleted, bool itemExistsInCart)
        {
            // Setup
            Cart cart = new Cart
            {
                ID = 1,
                Items = new List<CartItem>()
            };

            CartItemRequest request = new CartItemRequest { ItemID = 0, Count = 2 };
            
            if (userHasCart)
            {
                cart.Items = new List<CartItem>
                {
                    new CartItemBuilder().Random().Build(),
                    new CartItemBuilder().Random().Build()
                };
                if (itemExistsInCart)
                {
                    request.ItemID = cart.Items.ElementAt(0).ItemID;
                }
            }

            ShopUser user = new ShopUser
            {
                Id = "1"
            };

            Mock<ICartRepository> cartRepoMock = new Mock<ICartRepository>();
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<ILogger<CartController>> loggerMock = new Mock<ILogger<CartController>>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();

            cartRepoMock.Setup(o => o.FindByUserWithoutItemsData(It.IsAny<string>()))
                .ReturnsAsync(userHasCart ? cart : null);

            cartRepoMock.Setup(o => o.Insert(It.IsAny<Cart>()))
                .ReturnsAsync(cart)
                .Verifiable();

            userRepoMock.Setup(o => o.GetUserWithEmail(It.IsAny<string>()))
                .ReturnsAsync(user);

            itemRepoMock.Setup(o => o.FindByID(It.IsAny<long>()))
                .ReturnsAsync((long itemId) => { return itemFound ? new Item { ID = itemId, IsDeleted = itemDeleted } : null; });

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
            var result = await _controller.Put(request);

            // Assert
            if (!userHasCart)
            {
                cartRepoMock.Verify(o => o.Insert(It.IsAny<Cart>()));
            }
            if (!itemFound || itemDeleted)
            {
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);

        }
    }
}
