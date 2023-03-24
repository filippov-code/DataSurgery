﻿using System;
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
        //public abstract int Degree { get; }
        protected byte[] BytesForChange;
        protected int ChangeIndex = 0;
        public abstract long GetFreeSpace(int degree);

        //public abstract byte[] HideWithLSB(byte[] message);

        //public abstract byte[] FindLSB(int bytesCount);

        //public abstract byte[] ReadAllBytesLSB();

        public void AddWithLSB(byte[] message, int degree)
        {
            BitArray messageBits = new BitArray(message);
            for (int i = 0; i < messageBits.Count / degree; i++)
            {
                if (ChangeIndex == BytesForChange.Length)
                    break;

                for (int j = 0; j < degree; j++)
                {
                    BytesForChange[ChangeIndex] = SetBit(BytesForChange[ChangeIndex], 7 - j, messageBits[i * degree + j]);
                }
                ChangeIndex++;
            }
        }

        public byte[] FindLSB(int bytesCount, int degree, int startIndex)
        {
            bool[] values = new bool[bytesCount * 8];
            int bitsIndex = 0;
            for (int i = startIndex; i < BytesForChange.Length; i++)
            {
                if (bitsIndex == values.Length)
                    break;

                for (int j = 0; j < degree; j++)
                {
                    if (bitsIndex == values.Length)
                        break;

                    values[bitsIndex++] = GetBit(BytesForChange[i], 7 - j);
                }
            }
            byte[] result = new byte[bytesCount];
            BitArray bits = new BitArray(values);
            bits.CopyTo(result, 0);
            return result;
        }

        public abstract void Save(string path);

        protected static byte SetBit(byte value, int index, bool bit)
        {
            int tempMask = 1 << (7 - index);
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return (byte)result;
        }

        protected static bool GetBit(byte value, int index)
        {
            value >>= (7 - index);
            return Math.Abs(value % 2) == 1;
        }

        public enum FileFormats
        {
            Bmp = 1, Gif, Jpeg, Png, Tiff, Wav
        }

        public enum SurgeryMethods
        {
            Lsb
        }

        //public void WriteMessageLSB(byte[] container, byte[] message, int degree = 1, int increment = 1)
        //{
        //    //Console.WriteLine(@"\\\\\ HideWithLSB /////");
        //    //Console.WriteLine("bytes message lenght: " + message.Length);

        //    BitArray messageBits = new BitArray(message);
        //    //Console.WriteLine(BitConverter.IsLittleEndian);
        //    //foreach (bool bit in messageBits)
        //    //    Console.Write((bit? 1: 0) + " ");
        //    Console.WriteLine("message size: " + messageBits.Count);
        //    Console.WriteLine("space: " + container.Length * degree);
        //    if (messageBits.Count > container.Length * degree)
        //        throw new ArgumentException("Container too small");
        //    //Console.WriteLine("bits message length: " + messageBits.Count);
        //    int messageBitsIndex = 0;
        //    for (int containerIndex = 0; containerIndex < container.Length; containerIndex+=increment)
        //    {
        //        if (messageBitsIndex == messageBits.Length)
        //            break;
        //        for (int i = degree - 1; i >= 0; i--)
        //        {
        //            if (messageBitsIndex == messageBits.Length)
        //                break;

        //            container[containerIndex] = SetBit(container[containerIndex], 7 - i, messageBits[messageBitsIndex++]);
        //        }
        //    }
        //}

        //public byte[] ReadMessageLSB(byte[] container, int outBytesCount, int degree = 1, int increment = 1)
        //{
        //    bool[] values = new bool[outBytesCount * 8];
        //    int messageBitsIndex = 0;
        //    for (int containerIndex = 0; containerIndex < container.Length; containerIndex+=increment)
        //    {
        //        if (messageBitsIndex == values.Length)
        //            break;
        //        for (int i = degree - 1; i >= 0; i--)
        //        {
        //            if (messageBitsIndex == values.Length)
        //                break;

        //            values[messageBitsIndex++] = GetBit(container[containerIndex], 7-i);
        //        }
        //    }
        //    byte[] result = new byte[outBytesCount];
        //    BitArray bits = new BitArray(values);
        //    bits.CopyTo(result, 0);
        //    return result;
        //}

        //public void ReplaceElementsInArray<T>(T[] array, T[] elements, int startIndex)
        //{
        //    if (startIndex + elements.Length > array.Length)
        //        throw new ArgumentException();

        //    for (int i = 0; i < elements.Length; i++)
        //    {
        //        array[startIndex + i] = elements[i];
        //    }
        //}


        //public bool[] GetBitsWithInsignificantZeros(int value, int size)
        //{
        //    string bitsString = Convert.ToString(value, 2);
        //    if (bitsString.Length > size)
        //        throw new ArgumentException();
        //    return new bool[size - bitsString.Length]
        //        .Concat(bitsString.Select(x => x == '0' ? false : true))
        //        .ToArray();
        //}

    }
}
