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
    public class Post
    {
        CartController _controller;
        private readonly ITestOutputHelper _output;

        public Post(ITestOutputHelper output)
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

            CartRequest request = new CartRequest();
            request.Items = new List<CartItemRequest>
            {
                new CartItemRequest { ItemID = 1, Count = 1 },
                new CartItemRequest { ItemID = 2, Count = 2 }
            };

            if (userHasCart)
            {
                cart.Items = new List<CartItem>
                {
                    new CartItemBuilder().Random().Build(),
                    new CartItemBuilder().Random().Build()
                };
                if (itemExistsInCart)
                {
                    request.Items = new List<CartItemRequest>
                    {
                        new CartItemRequest { ItemID = cart.Items.ElementAt(0).ItemID, Count = 1 },
                        new CartItemRequest { ItemID = cart.Items.ElementAt(1).ItemID, Count = 2 }
                    };
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
            var result = await _controller.Post(request);

            // Assert
            if (!userHasCart)
            {
                cartRepoMock.Verify(o => o.Insert(It.IsAny<Cart>()));
            }
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            if (!itemFound)
            {
                var notFoundCount = Assert.IsType<int>(objectResult.Value);
                Assert.Equal(2, notFoundCount);
            }
        }
    }
}
