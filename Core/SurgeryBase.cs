using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core
{
    public abstract class SurgeryBase : ISurgery
    {
        public abstract int FreeSpace{ get; }
        public abstract byte[] HideWithLSB(byte[] message);
        public abstract byte[] FindLSB(byte[] ciphre);

        public void WriteMessageLSB(byte[] container, byte[] message, int degree = 1, int increment = 1)
        {
            //Console.WriteLine(@"\\\\\ HideWithLSB /////");
            //Console.WriteLine("bytes message lenght: " + message.Length);

            BitArray messageBits = new BitArray(message);
            //Console.WriteLine(BitConverter.IsLittleEndian);
            //foreach (bool bit in messageBits)
            //    Console.Write((bit? 1: 0) + " ");
            Console.WriteLine("message size: " + messageBits.Count);
            Console.WriteLine("space: " + container.Length * degree);
            if (messageBits.Count > container.Length * degree)
                throw new ArgumentException("Container too small");
            //Console.WriteLine("bits message length: " + messageBits.Count);
            int messageBitsIndex = 0;
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex+=increment)
            {
                if (messageBitsIndex == messageBits.Length)
                    break;
                for (int i = degree - 1; i >= 0; i--)
                {
                    if (messageBitsIndex == messageBits.Length)
                        break;

                    container[containerIndex] = SetBit(container[containerIndex], 7 - i, messageBits[messageBitsIndex++]);
                }
            }
        }

        public byte[] ReadMessageLSB(byte[] container, int outBytesCount, int degree = 1, int increment = 1)
        {
            bool[] values = new bool[outBytesCount * 8];
            int messageBitsIndex = 0;
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex+=increment)
            {
                if (messageBitsIndex == values.Length)
                    break;
                for (int i = degree - 1; i >= 0; i--)
                {
                    if (messageBitsIndex == values.Length)
                        break;

                    values[messageBitsIndex++] = GetBit(container[containerIndex], 7-i);
                }
            }
            byte[] result = new byte[outBytesCount];
            BitArray bits = new BitArray(values);
            bits.CopyTo(result, 0);
            return result;
        }

        public void ReplaceElementsInArray<T>(T[] array, T[] elements, int startIndex)
        {
            if (startIndex + elements.Length > array.Length)
                throw new ArgumentException();

            for (int i = 0; i < elements.Length; i++)
            {
                array[startIndex + i] = elements[i];
            }
        }

        public byte SetBit(byte value, int index, bool bit)
        {
            int tempMask = 1 << (7 - index);
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return (byte)result;
        }

        public static bool GetBit(byte value, int index)
        {
            value >>= (7 - index);
            return value % 2 == 1;
        }
        //
        //public byte[] CreateMETA(DSMD dsmd)
        //{
        //    if (dsmd.degree > 127 || degree < 0 ||
        //        sizeInBytes > Math.Pow(2, 4 * 8) - 1 || sizeInBytes < 0)
        //        throw new ArgumentException();

        //    bool[] frmt = GetBitsWithInsignificantZeros((int)format, 5);
        //    bool[] mthd = GetBitsWithInsignificantZeros((int)method, 4);
        //    bool[] dgre = GetBitsWithInsignificantZeros(degree, 7);
        //    bool[] mssz = GetBitsWithInsignificantZeros(sizeInBytes, 4 * 8);
        //    bool[] hstl = new BitArray(hash.TakeLast(2).ToArray()).Cast<bool>().ToArray();
        //    byte[] result = new byte[8];
        //    new BitArray(frmt.Concat(mthd).Concat(dgre).Concat(mssz).Concat(hstl).ToArray()).CopyTo(result, 0);
        //    return result;

        //}
        //public DSMD ReadMETA(byte[] meta)
        //{

        //}
        public bool[] GetBitsWithInsignificantZeros(int value, int size)
        {
            string bitsString = Convert.ToString(value, 2);
            if (bitsString.Length > size)
                throw new ArgumentException();
            return new bool[size - bitsString.Length]
                .Concat(bitsString.Select(x => x == '0' ? false : true))
                .ToArray();
        }
        //
        public enum FileFormats
        {
            Bmp = 1, Gif, Jpeg, Png, Tiff, Wav
        }

        public enum SurgeryMethods
        {
            Lsb = 1
        }
    }
}
