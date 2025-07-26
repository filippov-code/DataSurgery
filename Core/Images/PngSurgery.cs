using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Channels;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Core.BitmapImage
{
    public sealed class PngSurgery : PixelSurgeryBase
    {
        private Bitmap bitmap;

        public PngSurgery(string path)
        {
            bitmap = new Bitmap(path);
            BytesForChange = BitmapToBytes(bitmap).Select(x => (int)x).ToArray();
        }

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange.Select(x => (byte)x).ToArray(), bitmap);

            result.Save(path, ImageFormat.Png);
        }
    }
}
