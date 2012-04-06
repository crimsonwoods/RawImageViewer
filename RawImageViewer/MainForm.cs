using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;
using System.IO;

namespace RawImageViewer
{
    public partial class MainForm : Form
    {
        private Size imageSize = new Size(640, 480);
        private ImageLoader.PixelFormat pixelFormat = ImageLoader.PixelFormat.YUYV;
        private ImageFormatFilter[] imageFormats = new ImageFormatFilter[] {
            new ImageFormatFilter("PNG (*.png)", "*.png", ImageFormat.Png),
            new ImageFormatFilter("BMP (*.bmp)", "*.bmp", ImageFormat.Bmp),
            new ImageFormatFilter("JPEG (*.jpg)", "*.jpg", ImageFormat.Jpeg),
        };
        private string loadedImagePath = null;

        public MainForm()
        {
            InitializeComponent();

            var formats = ImageLoader.getSupportedPixelFormats();
            foreach (var f in formats)
            {
                ToolStripMenuItem item = new ToolStripMenuItem(f.ToString(), null, delegate(object sender, EventArgs args)
                {
                    selectSubItem(pixelFormatToolStripMenuItem.DropDownItems, (ToolStripMenuItem)sender);
                    pixelFormat = (ImageLoader.PixelFormat)((ToolStripMenuItem)sender).Tag;
                    updateStatus();
                    try
                    {
                        loadImage(loadedImagePath);
                    }
                    catch (Exception ex)
                    {
                        handleUncaughtException(ex);
                    }
                });
                item.Tag = f;
                pixelFormatToolStripMenuItem.DropDownItems.Add(item);
            }

            foreach (ToolStripMenuItem item in pixelFormatToolStripMenuItem.DropDownItems)
            {
                if ((ImageLoader.PixelFormat)item.Tag == pixelFormat)
                {
                    item.Checked = true;
                }
            }

            updateStatus();
        }

        private void openToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string path = null;

            using (OpenFileDialog dlg = new OpenFileDialog())
            {
                dlg.Multiselect = false;
                dlg.CheckFileExists = true;
                dlg.Filter = "All Files|*";
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    path = dlg.FileName;
                }
            }

            if (null != path)
            {
                try
                {
                    loadImage(path);
                    loadedImagePath = path;
                }
                catch (Exception ex)
                {
                    handleUncaughtException(ex);
                }
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (null == pictureBox.Image)
            {
                return;
            }

            string path = null;
            int filterIndex = -1;
            using (SaveFileDialog dlg = new SaveFileDialog())
            {
                dlg.Filter = createImageFormatFilter();
                dlg.FilterIndex = 0;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    path = dlg.FileName;
                    filterIndex = dlg.FilterIndex;
                }
            }

            if (null != path && -1 != filterIndex)
            {
                pictureBox.Image.Save(path, toImageFormat(filterIndex - 1));
            }
        }

        private void quitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void sizeToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (SizeDialog dlg = new SizeDialog())
            {
                dlg.Value = imageSize;
                if (dlg.ShowDialog(this) == DialogResult.OK)
                {
                    imageSize = dlg.Value;
                    updateStatus();
                    try
                    {
                        loadImage(loadedImagePath);
                    }
                    catch (Exception ex)
                    {
                        handleUncaughtException(ex);
                    }
                }
            }
        }

        private void versionToolStripMenuItem_Click(object sender, EventArgs e)
        {
            using (AboutBox box = new AboutBox())
            {
                box.ShowDialog(this);
            }
        }

        private void loadImage(string path)
        {
            if (string.IsNullOrEmpty(path))
            {
                return;
            }

            Image image = ImageLoader.Load(path, pixelFormat, imageSize.Width, imageSize.Height);
            if (null != image)
            {
                Image old = pictureBox.Image;
                pictureBox.Image = image;
                if (null != old)
                {
                    old.Dispose();
                }
            }
        }

        private void updateStatus()
        {
            toolStripStatusLabel.Text = string.Format("PixelFormat: {0}   Size: {1}x{2}", pixelFormat, imageSize.Width, imageSize.Height);
        }

        private void handleUncaughtException(Exception ex)
        {
            MessageBox.Show(this, ex.Message, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        private ImageFormat toImageFormat(int filterIndex)
        {
            return imageFormats[filterIndex].Format;
        }

        private string createImageFormatFilter()
        {
            StringBuilder builder = new StringBuilder();
            int length = imageFormats.Length;
            for (int i = 0; i < length; ++i)
            {
                if (i > 0)
                {
                    builder.Append("|");
                }
                builder.Append(imageFormats[i].Description).Append("|").Append(imageFormats[i].Extention);
            }
            return builder.ToString();
        }

        private void selectSubItem(ToolStripItemCollection container, ToolStripMenuItem target)
        {
            foreach (ToolStripMenuItem item in container)
            {
                if (item == target)
                {
                    item.Checked = true;
                }
                else
                {
                    item.Checked = false;
                }
            }
        }

        internal class ImageFormatFilter
        {
            public ImageFormatFilter(String filterDesc, String filterExt, ImageFormat imageFormat)
            {
                Description = filterDesc;
                Extention = filterExt;
                Format = imageFormat;
            }

            public String Description
            {
                get;
                set;
            }

            public String Extention
            {
                get;
                set;
            }

            public ImageFormat Format
            {
                get;
                set;
            }
        }
    }
}
