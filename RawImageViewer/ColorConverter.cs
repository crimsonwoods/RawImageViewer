using System;
using System.Collections.Generic;
using System.Text;

namespace RawImageViewer
{
    static class ColorConverter
    {
        public static int yuvtorgb(byte y, byte u, byte v)
        {
            byte b = (byte)(1.164 * (y - 16) + 2.018 * (u - 128));
            byte g = (byte)(1.164 * (y - 16) - 0.813 * (v - 128) - 0.391 * (u - 128));
            byte r = (byte)(1.164 * (y - 16) + 1.596 * (v - 128));
            return r << 16 | g << 8 | b;
        }
    }
}
