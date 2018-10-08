using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Text;

namespace eshopAPI.Tests.Helpers
{
    public static class ImageStream
    {
        public static Stream CreateImageStream()
        {
            List<Stream> list = new List<Stream>();
            Image img = RandomImage();
            MemoryStream ms = new MemoryStream();
            img.Save(ms, ImageFormat.Png);

            return ms;
        }

        static Image RandomImage()
        {
            int size = 240;
            Bitmap bitmap = new Bitmap(size, size);
            Random rand = new Random();
            for (int i = 0; i < size; i++)
            {
                for (int j = 0; j < size; j++)
                {
                    int r = rand.Next(256);
                    int g = rand.Next(256);
                    int b = rand.Next(256);
                    int a = rand.Next(256);

                    bitmap.SetPixel(i, j, Color.FromArgb(a, r, g, b));
                }
            }
            return bitmap;
        }
    }
}
