using eshop_webAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests.Account;
using eshopAPI.Services;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
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
    public class Login
    {
        AccountController _controller;
        private readonly ITestOutputHelper _output;

        public Login(ITestOutputHelper output)
        {
            _output = output;
        }

        [Fact]
        public async void AlreadySignedIn()
        {
            LoginRequest request = GetRequest();
            SetupController_AlreadySignedIn(request);

            var result = await _controller.Login(request);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void WrongEmail()
        {
            LoginRequest request = GetRequest();
            SetupController_WrongEmail(request);

            var result = await _controller.Login(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            var error = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal(ErrorReasons.InvalidEmailOrPassword, error.Reason);
        }

        [Fact]
        public async void BlockedAccount()
        {
            LoginRequest request = GetRequest();
            SetupController_BlockedAccount(request);

            var result = await _controller.Login(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.Forbidden, objectResult.StatusCode);
            var error = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal(ErrorReasons.UserIsBlocked, error.Reason);
        }

        [Fact]
        public async void EmailNotConfirmed()
        {
            LoginRequest request = GetRequest();
            SetupController_EmailNotConfirmed(request);

            var result = await _controller.Login(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            var error = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal(ErrorReasons.EmailNotConfirmed, error.Reason);
        }

        [Fact]
        public async void WrongPassword()
        {
            LoginRequest request = GetRequest();
            SetupController_WrongPassword(request);

            var result = await _controller.Login(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            var error = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal(ErrorReasons.BadRequest, error.Reason);
        }

        [Fact]
        public async void Success()
        {
            LoginRequest request = GetRequest();
            SetupController_Success(request);

            var result = await _controller.Login(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            var error = Assert.IsType<string>(objectResult.Value);
        }

        LoginRequest GetRequest()
        {
            return new LoginRequest
            {
                Email = "user@test.com",
                Password = "!23AbcD@",
                RememberMe = true
            };
        }

        void SetupController_AlreadySignedIn(LoginRequest request)
        {
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
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
        }

        void SetupController_WrongEmail(LoginRequest request)
        {
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, false);
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
        }

        void SetupController_BlockedAccount(LoginRequest request)
        {
            ShopUser user = new ShopUser
            {
                UserName = request.Email,
                Email = request.Email
            };

            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, false);
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
        }

        void SetupController_EmailNotConfirmed(LoginRequest request)
        {
            ShopUser user = new ShopUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = false
            };

            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user, false);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, false);
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
        }

        void SetupController_WrongPassword(LoginRequest request)
        {
            ShopUser user = new ShopUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user, false);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, false);
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
        }

        void SetupController_Success(LoginRequest request)
        {
            ShopUser user = new ShopUser
            {
                UserName = request.Email,
                Email = request.Email,
                EmailConfirmed = true
            };

            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager(user, false);
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object, false, true);
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
        }

        Mock<UserManager<ShopUser>> MockUserManager(ShopUser findByEmailResult = null, bool isUserBlocked = true)
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserEmailStore = mockUserStore.As<IUserEmailStore<ShopUser>>();
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ShopUser>>();
            
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Object.UserValidators.Add(new UserValidator<ShopUser>());
            userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<ShopUser>());
            userManagerMock.Setup(x => x.FindByEmailAsync(It.IsAny<string>()))
                .ReturnsAsync(findByEmailResult);
            userManagerMock.Setup(x => x.IsInRoleAsync(It.IsAny<ShopUser>(), UserRole.Blocked.ToString()))
                .ReturnsAsync(isUserBlocked);
            userManagerMock.Setup(x => x.GetRolesAsync(It.IsAny<ShopUser>()))
                .ReturnsAsync(new List<string> { "User" });

            return userManagerMock;
        }

        Mock<SignInManager<ShopUser>> MockSignInManager<ShopUser>(UserManager<ShopUser> userManager, bool isSignedIn = true, bool signInSuccessful = false) where ShopUser : class
        {
            var context = new Mock<HttpContext>();
            Mock<SignInManager<ShopUser>> signInManagerMock = new Mock<SignInManager<ShopUser>>(
                userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<ShopUser>>().Object,
                null, null, null)
            { CallBase = true };

            signInManagerMock.Setup(o => o.IsSignedIn(It.IsAny<ClaimsPrincipal>()))
                .Returns(isSignedIn);

            signInManagerMock.Setup(o => o.PasswordSignInAsync(It.IsAny<ShopUser>(), It.IsAny<string>(), It.IsAny<bool>(), false))
                .ReturnsAsync(signInSuccessful 
                    ? Microsoft.AspNetCore.Identity.SignInResult.Success 
                    : Microsoft.AspNetCore.Identity.SignInResult.Failed);
            return signInManagerMock;
        }
    }
}
