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
    public sealed class PngSurgery : BmpSurgery
    {
        private Bitmap bitmap;
        public readonly int FreeSpace;
        public readonly int Degree;

        public PngSurgery(string path, int degree = 1)
        {
            bitmap = new Bitmap(path);

            FreeSpace = bitmap.Width * bitmap.Height * 3;

            Degree = degree;
        }

        public override byte[] HideWithLSB(byte[] message)
        {
            Console.WriteLine(@"\\\\\ HideWithLSB /////");
            Console.WriteLine("bytes message lenght: " + message.Length);

            BitArray messageBits = new BitArray(message);
            Console.WriteLine("bits message length: " + messageBits.Count);

            int bitsIndex = 0;
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (bitsIndex == messageBits.Count)
                    break;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitsIndex == messageBits.Count)
                        break;

                    Color pixel = bitmap.GetPixel(x, y);
                    int r = pixel.R;
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == messageBits.Length)
                            break;

                        r = SetBit(r, 7 - i, messageBits[bitsIndex++]);
                    }
                    int g = pixel.G;
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == messageBits.Length)
                            break;

                        g = SetBit(g, 7 - i, messageBits[bitsIndex++]);
                    }
                    int b = pixel.B;
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == messageBits.Length)
                            break;

                        b = SetBit(b, 7 - i, messageBits[bitsIndex++]);
                    }
                    bitmap.SetPixel(x, y, Color.FromArgb(pixel.A, r, g, b));
                }
            }
            Console.WriteLine("bits hidden: " + bitsIndex);
            using (MemoryStream ms = new MemoryStream())
            {
                bitmap.Save(ms, ImageFormat.Png);
                return ms.ToArray();
            }
        }

        private int SetBit(int value, int index, bool bit)
        {
            int tempMask = 1 << 7 - index;
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return result;
        }

        public override byte[] FindLSB(int bitsCount)
        {
            Console.WriteLine(@"\\\\\ FindLSB /////");
            int bitsIndex = 0;
            bool[] messageValues = new bool[bitsCount];
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (bitsIndex == bitsCount)
                    break;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitsIndex == bitsCount)
                        break;

                    Color pixel = bitmap.GetPixel(x, y);
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.R, 7 - i);
                    }
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.G, 7 - i);
                    }
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.B, 7 - i);
                    }
                }
            }
            BitArray bits = new BitArray(messageValues);
            Console.WriteLine("bits finded: " + bits.Count);
            byte[] bytes = new byte[bits.Length / 8];
            bits.CopyTo(bytes, 0);
            Console.WriteLine("bytes finded: " + bytes.Length);
            return bytes;
        }

        public static bool GetBit(int value, int index)
        {
            value >>= 7 - index;
            return value % 2 == 1;
        }
    }
}
