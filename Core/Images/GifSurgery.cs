using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.BitmapImage
{
    public sealed class GifSurgery : BitsSurgeryBase
    {
        private FileInfo fileInfo;
        private byte[] fileBytes;
        private Bitmap bitmap;
        //byte[] bitmapBytes;
        private int paletteSize;
        //byte[] bytesForWrite;
        //private int degree;
        //public override int Degree => degree;

        public GifSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);
            bitmap = new Bitmap(path);
            //this.degree = degree;

            fileBytes = File.ReadAllBytes(fileInfo.FullName);
            int A2 = fileBytes[10];
            BitArray sizeBits = new BitArray
            (
                new bool[]
                {
                    A2 % 2 == 1,
                    (A2 >> 1) % 2 == 1,
                    (A2 >> 2) % 2 == 1
                }
             );
            int[] temp = new int[1];
            sizeBits.CopyTo(temp, 0);
            int PIXELflag = temp[0];
            //Console.WriteLine("PIXEL flag: " + PIXELflag);
            paletteSize = (int)Math.Pow(2, PIXELflag + 1);
            //Console.WriteLine("Colors count: " + colorsCount);
            BytesForChange = fileBytes[13..(13 + paletteSize * 3)];
        }

        public override long GetFreeSpace(int degree)
        {
            return paletteSize * 3 * degree;
        }

        public override void Save(string path)
        {
            byte[] result = fileBytes[..];
            Buffer.BlockCopy(BytesForChange, 0, result, 13, BytesForChange.Length);
            File.WriteAllBytes(path, result);
        }

        //public override byte[] HideWithLSB(byte[] message)
        //{
        //    WriteMessageInBytesLSB(bytesForWrite, message, Degree);
        //    Buffer.BlockCopy(bytesForWrite, 0, bitmapBytes, 13, paletteSize * 3);
        //    //ReplaceElementsInArray(bitmapBytes, bytesForWrite, 13);
        //    return bitmapBytes;
        //}

        //public override byte[] FindLSB(int bytesCount)
        //{
        //    return ReadMessageFromBytesLSB(bytesForWrite, bytesCount, Degree);
        //}

        //public override byte[] ReadAllBytesLSB()
        //{
        //    return ReadAllBytesFrombytesLSB(bytesForWrite, Degree);
        //}
    }
}
