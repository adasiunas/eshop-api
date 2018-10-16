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

namespace eshopAPI.Tests.Controllers.CheckoutControllerTests
{
    public class Checkout
    {
        CheckoutController _controller;

        public Checkout()
        {
        }

        [Theory]
        [InlineData(false, false)]
        [InlineData(true, false)]
        [InlineData(true, true)]
        public async void Success(bool userCartExists, bool paymentSuccessful)
        {
            // Setup
            CheckoutRequest request = new CheckoutRequest
            {
                Number = "abc",
                Address = new AddressBuilder().Random().Build()
            };
            Cart userCart = new Cart
            {
                ID = 1,
                Items = new List<CartItem>
                {
                    new CartItemBuilder().Random().Build(),
                    new CartItemBuilder().Random().Build()
                }
            };
            List<Discount> discounts = new List<Discount>
            {
                new DiscountBuilder().SetItem(userCart.Items.ElementAt(0).Item).Build()
            };
            ShopUser user = new ShopUser
            {
                Email = "test@test.com"
            };

            Mock<IPaymentService> paymentServiceMock = new Mock<IPaymentService>();
            Mock<ICartRepository> cartRepoMock = new Mock<ICartRepository>();
            Mock<IOrderRepository> orderRepoMock = new Mock<IOrderRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();

            cartRepoMock.Setup(o => o.FindByUser(It.IsAny<string>()))
                .ReturnsAsync(userCartExists ? userCart : null);

            discountRepoMock.Setup(o => o.GetDiscounts())
                .ReturnsAsync(discounts);

            userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            orderRepoMock.Setup(o => o.Insert(It.IsAny<Order>()))
                .ReturnsAsync((Order order) => order)
                .Verifiable();

            int ERROR_CODE = 404;
            paymentServiceMock.Setup(o => o.ProcessPaymentAsync(request))
                .ReturnsAsync(paymentSuccessful 
                    ? new PaymentResponse { IsSuccessfullResponse = true } 
                    : new PaymentResponse { IsSuccessfullResponse = false, ResponseCode = ERROR_CODE })
                .Verifiable();

            _controller = new CheckoutController(
                paymentServiceMock.Object,
                cartRepoMock.Object,
                orderRepoMock.Object,
                discountServiceMock.Object,
                discountRepoMock.Object,
                userManagerMock.Object,
                emailSenderMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Checkout(request);

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            if (!userCartExists)
            {
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }

            discountServiceMock.Verify(o => o.CalculateDiscountsForItems(It.IsAny<IEnumerable<CartItemVM>>(), discounts));
            orderRepoMock.Setup(o => o.Insert(It.IsAny<Order>()));

            if (!paymentSuccessful)
            {
                Assert.Equal(ERROR_CODE, objectResult.StatusCode);
                var paymentResponse = Assert.IsType<PaymentResponse>(objectResult.Value);
                return;
            }
            orderRepoMock.Verify(o => o.SaveChanges());
            emailSenderMock.Verify(o => o.SendOrderCreationEmailAsync(It.IsAny<string>(), It.IsAny<string>()));
            cartRepoMock.Verify(o => o.ClearCart(It.IsAny<Cart>()));
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        }

        Mock<UserManager<ShopUser>> MockUserManager()
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ShopUser>>();
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Object.UserValidators.Add(new UserValidator<ShopUser>());
            userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<ShopUser>());

            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ShopUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.AddToRoleAsync(It.IsAny<ShopUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Success);
            userManagerMock.Setup(x => x.GenerateEmailConfirmationTokenAsync(It.IsAny<ShopUser>()))
                .ReturnsAsync("code");

            return userManagerMock;
        }
    }
}
