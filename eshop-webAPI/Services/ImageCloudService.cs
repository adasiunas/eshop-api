using System;
using System.Collections.Generic;
using System.IO;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Services
{
    public interface IImageCloudService
    {
        List<Uri> UploadImagesFromFiles(IEnumerable<Stream> images);
        bool DeleteImage();
    }
    public class ImageCloudService : IImageCloudService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger _logger;
        private readonly Account _account;
        private readonly Cloudinary _cloudinary;

        public ImageCloudService(ILogger logger, IConfiguration configuration)
        {
            _logger = logger;
            _configuration = configuration;
            _account = new Account(
             _configuration["ImageCloudDetails:Name"], _configuration["ImageCloudDetails:APIkey"], _configuration["ImageCloudDetails:Secret"]    
            );
            _cloudinary = new Cloudinary(_account);
        }
        
        public List<Uri> UploadImagesFromFiles(IEnumerable<Stream> images)
        {
            var imagesUrls = new List<Uri>();
            foreach (var image in images)
            {
                var result = _cloudinary.Upload(new ImageUploadParams()
                {
                   File = new FileDescription(new Random().ToString(), image),
                   Async = "true"
                });
                imagesUrls.Add(result.Uri);
            }

            return imagesUrls;
        }

        public bool DeleteImage()
        {
            throw new System.NotImplementedException();
        }
    }
}