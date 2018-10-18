using eshop_webAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Services;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.Text;
using System.Threading;
using Xunit;

namespace eshopAPI.Tests.Controllers.AccountControllerTests
{
    public class RenewCsrfToken
    {
        AccountController _controller;

        public RenewCsrfToken()
        {
        }

        [Fact]
        public void Renew()
        {
            SetupController();
            var result = _controller.RenewCsrfToken();
            var noContentResult = Assert.IsType<NoContentResult>(result);
        }

        Mock<UserManager<ShopUser>> MockUserManager()
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            mockUserStore.Setup(x => x.FindByIdAsync(It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(new ShopUser()
                {
                    UserName = "test@email.com"
                });
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ShopUser>>();
            mockUserRoleStore.Setup(x => x.IsInRoleAsync(It.IsAny<ShopUser>(), It.IsAny<string>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);
            userManagerMock.Object.UserValidators.Add(new UserValidator<ShopUser>());
            userManagerMock.Object.PasswordValidators.Add(new PasswordValidator<ShopUser>());

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

        void SetupController()
        {
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<SignInManager<ShopUser>> signInManagerMock = MockSignInManager(userManagerMock.Object);
            Mock<IEmailSender> emailSenderMock = new Mock<IEmailSender>();
            Mock<ILogger<AccountController>> loggerMock = new Mock<ILogger<AccountController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>(); 
            configurationMock.SetupGet(x => x[It.Is<string>(o => o == "RedirectDomain")]).Returns("domain.com");

            _controller = new AccountController(userRepoMock.Object,
                userManagerMock.Object, 
                signInManagerMock.Object, 
                emailSenderMock.Object, 
                loggerMock.Object,
                configurationMock.Object);
        }
    }
}
