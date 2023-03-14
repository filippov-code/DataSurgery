using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public abstract class SurgeryBase
    {
        public void WriteMessageLSB(byte[] container, byte[] message, int degree = 1)
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
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex++)
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

        public byte[] ReadMessageLSB(byte[] container, int outBytesCount, int degree = 1)
        {
            bool[] values = new bool[outBytesCount * 8];
            int messageBitsIndex = 0;
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex++)
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
    }
}
