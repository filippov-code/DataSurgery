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
        //private int degree;
        //public override int Degree => degree;

        public PngSurgery(string path, int degree = 1)
        {
            bitmap = new Bitmap(path);
            //this.degree = degree;
            BytesForChange = BitmapToBytes(bitmap);
        }

        public override long GetFreeSpace(int degree)
        {
            return bitmap.Height * bitmap.Width * 3 * degree;
        }

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange, bitmap);

            result.Save(path, ImageFormat.Png);
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
