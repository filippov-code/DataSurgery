using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public abstract class PixelSurgeryBase : SurgeryBase
    {
        //protected Bitmap Bitmap { get; }

        protected static byte[] BitmapToBytes(Bitmap bitmap)
        {
            var result = new List<byte>();
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    result.Add(bitmap.GetPixel(x, y).R);
                    result.Add(bitmap.GetPixel(x, y).G);
                    result.Add(bitmap.GetPixel(x, y).B);
                }
            }
            return result.ToArray();
        }

        protected static Bitmap WriteBytesToPixels(byte[] bytes, Bitmap bitmap)
        {
            Queue<byte> q = new Queue<byte>(bytes);
            var result = new Bitmap(bitmap);
            //var result = new byte[bitmap.Width * bitmap.Height * 3];
            for (int y = 0; y < bitmap.Height; y++)
            {
                for (int x = 0; x < bitmap.Width; x++)
                {
                    byte r = q.Dequeue();
                    byte g = q.Dequeue();
                    byte b = q.Dequeue();
                    byte a = bitmap.GetPixel(x, y).A;
                    Color color = Color.FromArgb(a, r, g, b);
                    result.SetPixel(x, y, color);
                }
            }
            return result;
        }

        //protected static void WriteMessageInPixelsLSB(Bitmap bitmap, byte[] message, int degree)
        //{
        //    BitArray messageBits = new BitArray(message);

        //    int bitsIndex = 0;
        //    for (int y = 0; y < bitmap.Height; y++)
        //    {
        //        if (bitsIndex == messageBits.Count)
        //            break;

        //        for (int x = 0; x < bitmap.Width; x++)
        //        {
        //            if (bitsIndex == messageBits.Count)
        //                break;

        //            Color pixel = bitmap.GetPixel(x, y);
        //            int r = pixel.R;
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == messageBits.Length)
        //                    break;

        //                r = SetBit((byte)r, 7 - i, messageBits[bitsIndex++]);
        //            }
        //            int g = pixel.G;
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == messageBits.Length)
        //                    break;

        //                g = SetBit((byte)g, 7 - i, messageBits[bitsIndex++]);
        //            }
        //            int b = pixel.B;
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == messageBits.Length)
        //                    break;

        //                b = SetBit((byte)b, 7 - i, messageBits[bitsIndex++]);
        //            }
        //            bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
        //        }
        //    }
        //}

        //protected static byte[] ReadMessageFromPixelsLSB(Bitmap bitmap, int bytesCount ,int degree)
        //{
        //    //Console.WriteLine(@"\\\\\ FindLSB /////");
        //    int bitsIndex = 0;
        //    int bitsCount = bytesCount * 8;
        //    bool[] messageValues = new bool[bitsCount];
        //    for (int y = 0; y < bitmap.Height; y++)
        //    {
        //        if (bitsIndex == bitsCount)
        //            break;

        //        for (int x = 0; x < bitmap.Width; x++)
        //        {
        //            if (bitsIndex == bitsCount)
        //                break;

        //            Color pixel = bitmap.GetPixel(x, y);
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == bitsCount)
        //                    break;

        //                messageValues[bitsIndex++] = GetBit(pixel.R, 7 - i);
        //            }
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == bitsCount)
        //                    break;

        //                messageValues[bitsIndex++] = GetBit(pixel.G, 7 - i);
        //            }
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                if (bitsIndex == bitsCount)
        //                    break;

        //                messageValues[bitsIndex++] = GetBit(pixel.B, 7 - i);
        //            }
        //        }
        //    }
        //    BitArray bits = new BitArray(messageValues);
        //    //Console.WriteLine("bits finded: " + bits.Count);
        //    byte[] bytes = new byte[bytesCount];
        //    bits.CopyTo(bytes, 0);
        //    //Console.WriteLine("bytes finded: " + bytes.Length);
        //    return bytes;
        //}

        //public byte[] ReadAllBytesFromPixelsLSB(Bitmap bitmap, int degree)
        //{
        //    //Console.WriteLine(@"\\\\\ FindLSB /////");
        //    List<bool> messageValues = new List<bool>();
        //    for (int y = 0; y < bitmap.Height; y++)
        //    {
        //        for (int x = 0; x < bitmap.Width; x++)
        //        {
        //            Color pixel = bitmap.GetPixel(x, y);
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                messageValues.Add(GetBit(pixel.R, 7 - i));
        //            }
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                messageValues.Add(GetBit(pixel.G, 7 - i));
        //            }
        //            for (int i = degree - 1; i >= 0; i--)
        //            {
        //                messageValues.Add(GetBit(pixel.B, 7 - i));
        //            }
        //        }
        //    }
        //    BitArray bits = new BitArray(messageValues.ToArray());
        //    //Console.WriteLine("bits finded: " + bits.Count);
        //    byte[] bytes = new byte[bits.Count / 8];
        //    bits.CopyTo(bytes, 0);
        //    //Console.WriteLine("bytes finded: " + bytes.Length);
        //    return bytes;
        //}
    }
}
