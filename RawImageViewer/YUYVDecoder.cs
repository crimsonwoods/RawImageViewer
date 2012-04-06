using System;
using System.Collections.Generic;
using System.Text;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Runtime.InteropServices;

namespace RawImageViewer
{
    class YUYVDecoder : IImageDecoder
    {
        public Image Decode(Stream s, int width, int height)
        {
            if (null == s)
            {
                throw new ArgumentNullException("s");
            }

            if (0 != (width & 0x01))
            {
                throw new ArgumentException("parameter must be multiple of 2.", "width");
            }

            Bitmap image = new Bitmap(width, height, PixelFormat.Format32bppRgb);
            BitmapData data = image.LockBits(new Rectangle(new Point(), image.Size), ImageLockMode.WriteOnly, image.PixelFormat);
            try
            {
                try
                {
                    using (BinaryReader reader = new BinaryReader(s))
                    {
                        int ofs = 0;
                        int pix = 0;
                        for (int y = 0; y < height; ++y)
                        {
                            for (int x = 0; x < width; x += 2)
                            {
                                byte y1 = reader.ReadByte();
                                byte u = reader.ReadByte();
                                byte y2 = reader.ReadByte();
                                byte v = reader.ReadByte();

                                pix = ColorConverter.yuvtorgb(y1, u, v);
                                Marshal.WriteInt32(data.Scan0, ofs, pix);
                                ofs += 4;

                                pix = ColorConverter.yuvtorgb(y2, u, v);
                                Marshal.WriteInt32(data.Scan0, ofs, pix);
                                ofs += 4;
                            }
                        }
                    }
                }
                catch (EndOfStreamException)
                {
                }
                finally
                {
                    image.UnlockBits(data);
                }
            }
            catch (Exception ex)
            {
                image.Dispose();
                throw ex;
            }

            return image;
        }
    }
}
