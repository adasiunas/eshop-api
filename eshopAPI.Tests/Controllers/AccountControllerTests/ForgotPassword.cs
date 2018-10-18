using eshop_webAPI.Controllers;
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

namespace eshopAPI.Tests.Controllers.AccountControllerTests
{
    public class ForgotPassword
    {
        AccountController _controller;
        private readonly ITestOutputHelper _output;

        public ForgotPassword(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void ForgotPassword_Test(bool validUser)
        {
            ShopUser user = null;
            if (validUser)
                user = new ShopUser { Id = "a" };

            ForgotPasswordRequest request = new ForgotPasswordRequest
            {
                Email = "test@test.com"
            };
            // Setup
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            Mock<ILogger<AccountController>> loggerMock = new Mock<ILogger<AccountController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();

            emailSenderMock.Setup(o => o.SendResetPasswordEmailAsync(request.Email, It.IsAny<string>()))
                .Returns(Task.FromResult(0))
                .Verifiable();

            _controller = new AccountController(userRepoMock.Object,
                userManagerMock.Object,
                signInManagerMock.Object,
                emailSenderMock.Object,
                loggerMock.Object,
                configurationMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.ForgotPassword(request);

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
            emailSenderMock.Verify(o => o.SendResetPasswordEmailAsync(request.Email, It.IsAny<string>()));
        }
        
        Mock<UserManager<ShopUser>> MockUserManager(ShopUser user)
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserEmailStore = mockUserStore.As<IUserEmailStore<ShopUser>>();
            
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            userManagerMock.Setup(o => o.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(user);

            userManagerMock.Setup(o => o.GeneratePasswordResetTokenAsync(user))
                .ReturnsAsync("abc");

            return userManagerMock;
        }

        Mock<SignInManager<ShopUser>> MockSignInManager<ShopUser>(UserManager<ShopUser> userManager) where ShopUser : class
        {
            var context = new Mock<HttpContext>();
            Mock<SignInManager<ShopUser>> signInManagerMock = new Mock<SignInManager<ShopUser>>(
                userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<ShopUser>>().Object,
                null, null, null)
            { CallBase = true };

            return signInManagerMock;
        }
    }
}
