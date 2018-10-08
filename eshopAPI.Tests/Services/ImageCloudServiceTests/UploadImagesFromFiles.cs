using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using eshopAPI.Services;
using eshopAPI.Tests.Helpers;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Moq;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Xunit;

namespace eshopAPI.Tests.Services.ImageCloudServiceTests
{
    public class UploadImagesFromFiles
    {
        ImageCloudServiceCloudinaryMock _imageCloudService;

        public UploadImagesFromFiles()
        {
            var loggerMock = new Mock<ILogger<IImageCloudService>>();
            var configurationMock = new Mock<IConfiguration>();
            configurationMock.SetupGet(x => x[It.Is<string>(o => o == "ImageCloudDetails:Name")]).Returns("Name");
            configurationMock.SetupGet(x => x[It.Is<string>(o => o == "ImageCloudDetails:APIkey")]).Returns("ApiKey");
            configurationMock.SetupGet(x => x[It.Is<string>(o => o == "ImageCloudDetails:Secret")]).Returns("Secret");
            _imageCloudService = new ImageCloudServiceCloudinaryMock(loggerMock.Object, configurationMock.Object);
        }

        [Fact]
        public void Method()
        {
            List<Stream> images = new List<Stream>();
            images.Add(ImageStream.CreateImageStream());
            List<Uri> uri = _imageCloudService.UploadImagesFromFiles(images);
            uri.ForEach(Console.WriteLine);

            Assert.Equal(images.Count, uri.Count);
        }

        class ImageCloudServiceCloudinaryMock : ImageCloudService
        {
            public ImageCloudServiceCloudinaryMock(ILogger<IImageCloudService> logger, IConfiguration configuration)
                :base(logger, configuration)
            {
            }

            protected override RawUploadResult CloudinaryUpload(RawUploadParams param)
            {
                return new RawUploadResult();
            }
        }
    }
}
