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
using System.Text;
using System.Threading;
using Xunit;

namespace eshopAPI.Tests.Controllers.AccountControllerTests
{
    public class Register
    {
        AccountController _controller;

        public Register()
        {
        }

        [Fact]
        public async void Success()
        {
            RegisterRequest request = new RegisterRequest
            {
                Username = "user@test.com",
                Password = "!23AbcD@",
                ConfirmPassword = "!23AbcD@"
            };
            Success_SetupController(request);

            var result = await _controller.Register(request);
            var statusCodeResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusCodeResult.StatusCode);
        }

        [Fact]
        public async void Failure()
        {
            RegisterRequest request = new RegisterRequest
            {
                Username = "user@test.com",
                Password = "!23AbcD@",
                ConfirmPassword = "!23AbcD@"
            };
            Failure_SetupController(request);

            var result = await _controller.Register(request);
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.BadRequest, objectResult.StatusCode);
            var error = Assert.IsType<ErrorResponse>(objectResult.Value);
            Assert.Equal("No user created", error.Message);
        }

        void Success_SetupController(RegisterRequest request)
        {
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManageForSuccess();
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
        }

        Mock<UserManager<ShopUser>> MockUserManageForSuccess()
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

        Mock<SignInManager<ShopUser>> MockSignInManager<ShopUser>(UserManager<ShopUser> userManager) where ShopUser : class
        {
            var context = new Mock<HttpContext>();
            return new Mock<SignInManager<ShopUser>>(
                userManager,
                new HttpContextAccessor { HttpContext = context.Object },
                new Mock<IUserClaimsPrincipalFactory<ShopUser>>().Object,
                null, null, null)
            { CallBase = true };
        }


        void Failure_SetupController(RegisterRequest request)
        {
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManageForFailure();
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
        }

        Mock<UserManager<ShopUser>> MockUserManageForFailure()
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ShopUser>>();
            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Object.UserValidators.Add(new UserValidator<ShopUser>());
            userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<ShopUser>());
            userManagerMock.Setup(x => x.CreateAsync(It.IsAny<ShopUser>(), It.IsAny<string>()))
                .ReturnsAsync(IdentityResult.Failed(new IdentityError() { Code = "1", Description = "No user created"}));
            return userManagerMock;
        }
    }
}
