using Microsoft.AspNetCore.Hosting;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace eshopAPI.Services
{
    public class ImageLocalStorageDecorator : IImageCloudService
    {
        private IImageCloudService _parent;
        private IHostingEnvironment _hostingEnvironment;
        public ImageLocalStorageDecorator(
            IImageCloudService parent,
            IHostingEnvironment hostingEnvironment
            )
        {
            _parent = parent;
            _hostingEnvironment = hostingEnvironment;
        }

        public List<Uri> UploadImagesFromFiles(IEnumerable<Stream> images)
        {
            int index = 0;
            foreach(Stream stream in images){
                DateTime time = DateTime.Now;
                using (var fileStream = new FileStream(Path.Combine(_hostingEnvironment.ContentRootPath, "Pictures", $"ImportPicture-{time.Year}-{time.Month}-{time.Day}-{time.Hour}-{time.Minute}-{time.Second}-{index}.png"), FileMode.Create, FileAccess.Write))
                {
                    stream.CopyTo(fileStream);
                }
                index++;
            }

            return _parent.UploadImagesFromFiles(images);
        }
    }
}
