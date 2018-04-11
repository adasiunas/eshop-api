using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using CloudinaryDotNet;
using CloudinaryDotNet.Actions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace eshopAPI.Services
{
    public interface IImageCloudService
    {
        List<Uri> UploadImagesFromFiles(IEnumerable<Stream> images);
    }
    public class ImageCloudService : IImageCloudService
    {
        private readonly IConfiguration _configuration;
        private readonly ILogger<IImageCloudService> _logger;
        private readonly Account _account;
        private readonly Cloudinary _cloudinary;

        public ImageCloudService(ILogger<IImageCloudService> logger, IConfiguration configuration)
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
            _logger.LogInformation($"Starting to upload {images.Count()} images to Cloud");
            var imagesUrls = new List<Uri>();
            foreach (var image in images)
            {
                image.Position = 0;
                var uploadParam = new RawUploadParams()
                {
                    File = new FileDescription(new Random().ToString(), image)    
                };
                var result = _cloudinary.Upload(uploadParam);
                
                if (result.Error != null)
                {
                    _logger.LogWarning($"Failed to upload image. Error: {result.Error.Message}");
                }
                else
                {
                    imagesUrls.Add(result.SecureUri);
                    _logger.LogInformation($"Image was successfully uploaded to the cloud. Url: {result.SecureUri}");
                }
            }

            return imagesUrls;
        }
    }
}