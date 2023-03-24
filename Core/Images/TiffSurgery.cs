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
        //private int degree;
        //public override int Degree => degree;

        public TiffSurgery(string path, int degree = 1)
        {
            bitmap = new Bitmap(path);

            BytesForChange = BitmapToBytes(bitmap);
        }

        public override long GetFreeSpace(int degree)
        {
            return bitmap.Height * bitmap.Width * 3 * degree;
        }

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange, bitmap);

            result.Save(path, ImageFormat.Tiff);
        }

        //public override byte[] HideWithLSB(byte[] message)
        //{
        //    WriteMessageInPixelsLSB(bitmap, message, degree);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        bitmap.Save(ms, ImageFormat.Png);
        //        return ms.ToArray();
        //    }
        //}
        //public override byte[] FindLSB(int bytesCount)
        //{
        //    return ReadMessageFromPixelsLSB(bitmap, bytesCount, degree);
        //}
        //public override byte[] ReadAllBytesLSB()
        //{
        //    return ReadAllBytesFromPixelsLSB(bitmap, Degree);
        //}
    }
}
