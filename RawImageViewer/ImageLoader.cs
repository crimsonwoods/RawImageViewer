using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace RawImageViewer
{
    static class ImageLoader
    {
        public enum PixelFormat
        {
            YUYV,
            UYVY,
        }

        private static Dictionary<PixelFormat, IImageDecoder> decoders = new Dictionary<PixelFormat, IImageDecoder>()
        {
            { PixelFormat.YUYV, new YUYVDecoder() },
            { PixelFormat.UYVY, new UYVYDecoder() },
        };

        public static IEnumerable<PixelFormat> getSupportedPixelFormats()
        {
            return decoders.Keys;
        }

        public static Image Load(string path, PixelFormat format, int width, int height)
        {
            if (!decoders.ContainsKey(format))
            {
                throw new NotSupportedException("Unsupported pixel format.");
            }

            return decoders[format].Decode(File.OpenRead(path), width, height);
        }
    }
}
