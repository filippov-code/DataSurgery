using Core.Interfaces;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.BitmapImage
{
    public sealed class BmpSurgery : PixelSurgeryBase
    {
        private Bitmap bitmap;

        public BmpSurgery(string path)
        {
            bitmap = new Bitmap(path);

            BytesForChange = BitmapToBytes(bitmap).Select(x => (int)x).ToArray();
        }

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange.Select(x => (byte)x).ToArray(), bitmap);

            result.Save(path, ImageFormat.Bmp);
        }
    }
}