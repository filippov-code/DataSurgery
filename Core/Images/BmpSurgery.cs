using Core.Interfaces;
using System.Collections;
using System.Drawing;
using System.Drawing.Imaging;

namespace Core.BitmapImage
{
    public sealed class BmpSurgery : PixelSurgeryBase
    {
        private Bitmap bitmap;
        //private int degree;
        //public override int Degree => degree;
        //private byte[] BytesForChange;
        //private int ChangeIndex = 0;

        public BmpSurgery(string path, int degree)
        {
            //this.degree = degree;
            bitmap = new Bitmap(path);

            BytesForChange = BitmapToBytes(bitmap);
        }

        //public void AddWithLSB(byte[] message, int degree)
        //{
        //    BitArray messageBits = new BitArray(message);
        //    for (int i = 0; i < messageBits.Count / degree; i++)
        //    {
        //        if (currentComponentsIndex == colorComponents.Length)
        //            break;

        //        for (int j = 0; j < degree; j++)
        //        {
        //            colorComponents[currentComponentsIndex] = SetBit(colorComponents[currentComponentsIndex], 7 - j, messageBits[i * degree + j]);
        //        }
        //        currentComponentsIndex++;
        //    }
        //}

        //public byte[] FindLSB(int bytesCount, int degree, int startIndex)
        //{
        //    bool[] values = new bool[bytesCount * 8];
        //    int bitsIndex = 0;
        //    for (int i = startIndex; i < colorComponents.Length; i++)
        //    {
        //        if (bitsIndex == values.Length)
        //            break;

        //        for (int j = 0; j < degree; j++)
        //        {
        //            if (bitsIndex == values.Length)
        //                break;

        //            values[bitsIndex++] = GetBit(colorComponents[i], 7 - j);
        //        }
        //    }
        //    byte[] result = new byte[bytesCount];
        //    BitArray bits = new BitArray(values);
        //    bits.CopyTo(result, 0);
        //    return result;
        //}

        public override void Save(string path)
        {
            var result = WriteBytesToPixels(BytesForChange, bitmap);

            result.Save(path, ImageFormat.Bmp);
        }

        public override long GetFreeSpace(int degree)
        {
            return bitmap.Height * bitmap.Width * 3 * degree;
        }

        //public override byte[] HideWithLSB(byte[] message)
        //{
        //    WriteMessageInPixelsLSB(bitmap, message, degree);

        //    using (MemoryStream ms = new MemoryStream())
        //    {
        //        bitmap.Save(ms, ImageFormat.Bmp);
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