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
    public class Logout
    {
        AccountController _controller;
        private readonly ITestOutputHelper _output;

        public Logout(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(true)]
        [InlineData(false)]
        public async void TryLogout(bool isSignedId)
        {
            // Setup
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, isSignedId);
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            Mock<ILogger<AccountController>> loggerMock = new Mock<ILogger<AccountController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();

            _controller = new AccountController(userRepoMock.Object,
                userManagerMock.Object,
                signInManagerMock.Object,
                emailSenderMock.Object,
                loggerMock.Object,
                configurationMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Logout();

            // Assert
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
            if (isSignedId)
            {
                signInManagerMock.Verify(o => o.SignOutAsync());
            }
        }
        
        Mock<UserManager<ShopUser>> MockUserManager()
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserEmailStore = mockUserStore.As<IUserEmailStore<ShopUser>>();
            
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            return userManagerMock;
        }

        Mock<SignInManager<ShopUser>> MockSignInManager<ShopUser>(UserManager<ShopUser> userManager, bool isSignedIn) where ShopUser : class
        {
            var context = new Mock<HttpContext>();
            Mock<SignInManager<ShopUser>> signInManagerMock = new Mock<SignInManager<ShopUser>>(
                userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<ShopUser>>().Object,
                null, null, null)
            { CallBase = true };

            var auth = new Mock<IAuthenticationService>();
            
            signInManagerMock.Setup(o => o.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(isSignedIn);

            signInManagerMock.Setup(o => o.SignOutAsync())
                .Returns(Task.FromResult(0))
                .Verifiable();

            return signInManagerMock;
        }
    }
}
