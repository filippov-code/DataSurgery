using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.BitmapImage
{
    public sealed class TiffSurgery : PixelSurgeryBase
    {
        private Bitmap bitmap;

        public TiffSurgery(string path)
        {
            bitmap = new Bitmap(path);

            BytesForChange = BitmapToBytes(bitmap).Select(x => (int)x).ToArray();
        }

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange.Select(x => (byte)x).ToArray(), bitmap);

            result.Save(path, ImageFormat.Tiff);
        }
    }
}
