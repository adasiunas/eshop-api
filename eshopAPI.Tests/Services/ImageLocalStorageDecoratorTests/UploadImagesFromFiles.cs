using eshopAPI.Services;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using Moq;
using Xunit;
using Microsoft.AspNetCore.Hosting;
using System.Drawing;
using System.Drawing.Imaging;
using eshopAPI.Tests.Helpers;

namespace eshopAPI.Tests.Services.ImageLocalStorageDecoratorTests
{
    public class UploadImagesFromFiles
    {
        IImageCloudService _imageLocalStorageDecorator;
        string _picturesDirectory;

        public UploadImagesFromFiles()
        {
            var currentDirectory = Path.Combine(Directory.GetCurrentDirectory(), "..", "..", "..");
            _picturesDirectory = Path.Combine(currentDirectory, "Pictures");
            if (!Directory.Exists(_picturesDirectory))
                Directory.CreateDirectory(_picturesDirectory);
            CleanDirectory();

            var imageService = new Mock<IImageCloudService>();
            imageService.Setup(o => o.UploadImagesFromFiles(new List<Stream>())).Returns(new List<Uri>());
            var hostingEnvironment = new Mock<IHostingEnvironment>();
            
            hostingEnvironment.Setup(foo => foo.ContentRootPath).Returns(currentDirectory);

            _imageLocalStorageDecorator = new ImageLocalStorageDecorator(imageService.Object, hostingEnvironment.Object);
        }
        [Fact]
        public void Upload()
        {
            List<Stream> images = new List<Stream>();
            images.Add(ImageStream.CreateImageStream());
            _imageLocalStorageDecorator.UploadImagesFromFiles(images);

            string[] files = Directory.GetFiles(_picturesDirectory, "*.png");

            Assert.Single(files);
        }


        void CleanDirectory()
        {
            DirectoryInfo di = new DirectoryInfo(_picturesDirectory);

            foreach (FileInfo file in di.GetFiles())
            {
                file.Delete();
            }
        }
    }
}
