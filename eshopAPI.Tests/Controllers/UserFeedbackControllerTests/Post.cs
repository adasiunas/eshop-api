using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
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

namespace eshopAPI.Tests.Controllers.UserFeedbackControllerTests
{
    public class Post
    {
        UserFeedbackController _controller;
        private readonly ITestOutputHelper _output;

        public Post(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void Post_Test(bool validUser)
        {
            // Setup
            ShopUser user = new ShopUser
            {
                Id = "1"
            };

            UserFeedbackRequest request = new UserFeedbackRequest
            {
                Rating = 9,
                Message = "Very good"
            };

            Mock<IUserFeedbackRepository> feedbackRepoMock = new Mock<IUserFeedbackRepository>();
            Mock<UserManager<ShopUser>> userManagerMock = MockUserManager();
            Mock<ILogger<UserFeedbackController>> loggerMock = new Mock<ILogger<UserFeedbackController>>();

            userManagerMock.Setup(o => o.FindByNameAsync(It.IsAny<string>()))
                .ReturnsAsync(validUser ? user : null);

            feedbackRepoMock.Setup(o => o.Insert(It.IsAny<UserFeedbackEntry>()))
                .ReturnsAsync((UserFeedbackEntry input) => { return input; })
                .Verifiable();

            _controller = new UserFeedbackController(
                feedbackRepoMock.Object,
                userManagerMock.Object,
                loggerMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Post(request);

            // Assert
            if (!validUser)
            {
                var objectResult = Assert.IsType<ObjectResult>(result);
                Assert.Equal((int)HttpStatusCode.NotFound, objectResult.StatusCode);
                var errorResponse = Assert.IsType<ErrorResponse>(objectResult.Value);
                Assert.Equal(ErrorReasons.NotFound, errorResponse.Reason);
                return;
            }
            feedbackRepoMock.Verify(o => o.Insert(It.IsAny<UserFeedbackEntry>()));
            var statusResult = Assert.IsType<StatusCodeResult>(result);
            Assert.Equal((int)HttpStatusCode.NoContent, statusResult.StatusCode);
        }

        Mock<UserManager<ShopUser>> MockUserManager()
        {
            var mockUserStore = new Mock<IUserStore<ShopUser>>();
            var mockUserEmailStore = mockUserStore.As<IUserEmailStore<ShopUser>>();
            var mockUserRoleStore = mockUserStore.As<IUserRoleStore<ShopUser>>();

            var userManagerMock = new Mock<UserManager<ShopUser>>(mockUserStore.Object, null, null, null, null, null, null, null, null);

            return userManagerMock;
        }
    }
}
