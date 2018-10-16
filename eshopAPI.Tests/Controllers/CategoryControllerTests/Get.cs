using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Models.ViewModels;
using eshopAPI.Requests.Account;
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

namespace eshopAPI.Tests.Controllers.CategoryControllerTests
{
    public class Get
    {
        CategoryController _controller;
        private readonly ITestOutputHelper _output;

        public Get(ITestOutputHelper output)
        {
            _output = output;
        }

        [Theory]
        [InlineData(false)]
        [InlineData(true)]
        public async void Get_Test(bool validUser)
        {
            // Setup
            Mock<ILogger<CategoryController>> loggerMock = new Mock<ILogger<CategoryController>>();
            Mock<ICategoryRepository> categoryRepoMock = new Mock<ICategoryRepository>();

            List<Category> categories = new List<Category>
            {
                new CategoryBuilder().Build(),
                new CategoryBuilder().Build(),
                new CategoryBuilder().Build()
            };
            categoryRepoMock.Setup(o => o.GetCategoriesWithSubcategories())
                .ReturnsAsync(categories);

            _controller = new CategoryController(
                loggerMock.Object,
                categoryRepoMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            // Act
            var result = await _controller.Get();

            // Assert
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            try
            {
                IEnumerable<CategoryVM> response = (IEnumerable<CategoryVM>)objectResult.Value;
                Assert.Equal(3, response.ToList().Count);
            } catch
            {
                Assert.True(false);
            }
        }
    }
}
