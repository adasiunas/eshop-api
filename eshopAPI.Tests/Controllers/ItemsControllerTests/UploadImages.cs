using eshop_webAPI.Controllers;
using eshopAPI.Controllers;
using eshopAPI.DataAccess;
using eshopAPI.Models;
using eshopAPI.Requests;
using eshopAPI.Requests.Account;
using eshopAPI.Response;
using eshopAPI.Services;
using eshopAPI.Tests.Builders;
using eshopAPI.Tests.Helpers;
using eshopAPI.Utils;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Internal;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using Xunit;

namespace eshopAPI.Tests.Controllers.ItemsControllerTests
{
    public class UploadImages
    {
        ItemsController _controller;

        public UploadImages()
        {
        }

        [Fact]
        public async void Success()
        {
            Mock<ILogger<ItemsController>> loggerMock = new Mock<ILogger<ItemsController>>();
            Mock<IConfiguration> configurationMock = new Mock<IConfiguration>();
            Mock<IImageCloudService> imageServiceMock = new Mock<IImageCloudService>();
            Mock<IItemRepository> itemRepoMock = new Mock<IItemRepository>();
            Mock<IDiscountRepository> discountRepoMock = new Mock<IDiscountRepository>();
            Mock<IPaymentService> paymentServiceMock = new Mock<IPaymentService>();
            Mock<ICategoryRepository> categoryRepoMock = new Mock<ICategoryRepository>();
            Mock<IDiscountService> discountServiceMock = new Mock<IDiscountService>();
            Mock<IHostingEnvironment> hostingEnvMock = new Mock<IHostingEnvironment>();

            imageServiceMock.Setup(o => o.UploadImagesFromFiles(It.IsAny<List<Stream>>()))
                .Returns((List<Stream> img) => { return img.Select(o => new Uri("https://www.cloud.com")).ToList(); })
                .Verifiable();

            _controller = new ItemsController(
                loggerMock.Object,
                configurationMock.Object,
                imageServiceMock.Object,
                itemRepoMock.Object,
                discountRepoMock.Object,
                paymentServiceMock.Object,
                categoryRepoMock.Object,
                discountServiceMock.Object,
                hostingEnvMock.Object);

            _controller.ControllerContext = new ControllerContext();
            _controller.ControllerContext.HttpContext = new DefaultHttpContext();

            Stream imageStream = ImageStream.CreateImageStream();
            IEnumerable<IFormFile> images = new List<FormFile>
            {
                new FormFile(imageStream, 0, imageStream.Length, "name", "filename")
            };

            // Act
            var result = await _controller.UploadImages(images);

            // Assert
            imageServiceMock.Verify(o => o.UploadImagesFromFiles(It.IsAny<List<Stream>>()));
            var objectResult = Assert.IsType<ObjectResult>(result);
            Assert.Equal((int)HttpStatusCode.OK, objectResult.StatusCode);
            var pictureUris = Assert.IsType<Uri[]>(objectResult.Value);
            Assert.Single(pictureUris);
        }
    }
}
