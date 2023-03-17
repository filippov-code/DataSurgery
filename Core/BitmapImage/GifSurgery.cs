using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BitmapImage
{
    public class GifSurgery : SurgeryBase
    {
        private FileInfo fileInfo;
        private Bitmap bitmap;
        public readonly int FreeSpace;
        public readonly int Degree;

        public List<short> tmpBeforeSave = new();
        public List<short> tmpAfterSave = new();

        public GifSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);
            bitmap = new Bitmap(path);

            //FreeSpace = bitmap.Width * bitmap.Height * 3;

            Degree = degree;
        }

        public byte[] HideWithLSB(byte[] message)
        {
            byte[] bitmapBytes = File.ReadAllBytes(fileInfo.FullName);
            int a = bitmapBytes[10];
            BitArray sizeBits = new BitArray
            (
                new bool[]
                {
                    a % 2 == 1,
                    (a >> 1) % 2 == 1,
                    (a >> 2) % 2 == 1
                }
             );
            int[] temp = new int[1];
            sizeBits.CopyTo(temp, 0);
            int PIXELflag = temp[0];
            //Console.WriteLine("PIXEL flag: " + PIXELflag);
            int colorsCount = (int)Math.Pow(2, PIXELflag + 1);
            //Console.WriteLine("Colors count: " + colorsCount);
            byte[] bytesForWrite = bitmapBytes[13..(13 + colorsCount * 3)];
            //Console.WriteLine("Bytes to write: " + string.Join(" ", bytesForWrite));
            //Console.WriteLine("Check: " + (bytesForWrite.Length % 3 == 0));
            //Console.WriteLine(bytesForWrite.Length);
            WriteMessageLSB(bytesForWrite, message, Degree);
            //Console.WriteLine(bytesForWrite.Length);
            //bitmapBytes[13..(13 + colorsCount * 3)] = bytesForWrite;
            ReplaceElementsInArray(bitmapBytes, bytesForWrite, 13);
            return bitmapBytes;
        }

        private int SetBit(int value, int index, bool bit)
        {
            int tempMask = 1 << 7 - index;
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return result;
        }

        public byte[] FindLSB(int bitsCount)
        {
            byte[] bitmapBytes = File.ReadAllBytes(fileInfo.FullName);
            int a = bitmapBytes[10];
            BitArray sizeBits = new BitArray
            (
                new bool[]
                {
                    a % 2 == 1,
                    (a >> 1) % 2 == 1,
                    (a >> 2) % 2 == 1
                }
             );
            int[] temp = new int[1];
            sizeBits.CopyTo(temp, 0);
            int PIXELflag = temp[0];
            //Console.WriteLine("PIXEL flag: " + PIXELflag);
            int colorsCount = (int)Math.Pow(2, PIXELflag + 1);
            //Console.WriteLine("Colors count: " + colorsCount);
            byte[] bytesForWrite = bitmapBytes[13..(13 + colorsCount * 3)];
            return ReadMessageLSB(bytesForWrite, bitsCount / 8, Degree);

        }

        public static bool GetBit(int value, int index)
        {
            value >>= 7 - index;
            return Math.Abs(value % 2) == 1;
        }
    }
}
