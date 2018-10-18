using eshop_webAPI.Controllers;
using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Requests.Account;
using eshopAPI.Requests.User;
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

namespace eshopAPI.Tests.Controllers.UserControllerTests
{
    public class UpdateUser
    {
        UserController _controller;

        public UpdateUser()
        {
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void Success(bool validUser)
        {
            // Setup 
            ShopUser user = new ShopUser
            {
                Id = "1"
            };
            UpdateUserRequest updateUserRequest = new UpdateUserRequest
            {
                Name = "Test",
                Surname = "Test",
                Phone = "123456789",
                Address = new AddressBuilder().Random().Build()
            };

            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<ILogger<UserController>> loggerMock = new Mock<ILogger<UserController>>();

            userRepoMock.Setup(o => o.GetUserWithEmail(It.IsAny<string>()))
                .ReturnsAsync(validUser ? user : null);

            _controller = new UserController(
                userRepoMock.Object,
                loggerMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.UpdateUser(updateUserRequest);

            // Assert
            if (!validUser)
            {
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }

            userRepoMock.Verify(o => o.UpdateUserProfile(user, updateUserRequest));
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);
        }
    }
}
