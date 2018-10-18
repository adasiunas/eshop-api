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

namespace eshopAPI.Tests.Controllers.UserControllerTests
{
    public class Profile
    {
        UserController _controller;

        public Profile()
        {
        }

        [Fact]
        public async void Success()
        {
            // Setup 
            Mock<IShopUserRepository> userRepoMock = new Mock<IShopUserRepository>();
            Mock<ILogger<UserController>> loggerMock = new Mock<ILogger<UserController>>();

            _controller = new UserController(
                userRepoMock.Object,
                loggerMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Profile();

            // Assert
            userRepoMock.Verify(o => o.GetUserProfile(It.IsAny<string>()));
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
        }
    }
}
