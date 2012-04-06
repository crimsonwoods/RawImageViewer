using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.IO;

namespace RawImageViewer
{
    interface IImageDecoder
    {
        Image Decode(Stream s, int width, int height);
    }
}
