using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class GifSurgery
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
            int blockPIXEL = temp[0];
            Console.WriteLine(blockPIXEL);
            

            return null;
        }

        private int SetBit(int value, int index, bool bit)
        {
            int tempMask = 1 << (7 - index);
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return result;
        }

        public byte[] FindLSB(int bitsCount)
        {
            return null;
        }

        public static bool GetBit(int value, int index)
        {
            value >>= (7 - index);
            return Math.Abs(value % 2) == 1;
        }
    }
}
