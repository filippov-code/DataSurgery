using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public abstract class BitsSurgeryBase : SurgeryBase
    {
        protected static void WriteMessageInBytesLSB(byte[] container, byte[] message, int degree = 1, int increment = 1)
        {
            //Console.WriteLine(@"\\\\\ HideWithLSB /////");
            //Console.WriteLine("bytes message lenght: " + message.Length);

            BitArray messageBits = new BitArray(message);
            //Console.WriteLine(BitConverter.IsLittleEndian);
            //foreach (bool bit in messageBits)
            //    Console.Write((bit? 1: 0) + " ");
            //Console.WriteLine("message size: " + messageBits.Count);
            //Console.WriteLine("space: " + container.Length * degree);
            if (messageBits.Count > container.Length * degree)
                throw new ArgumentException("Container too small");
            //Console.WriteLine("bits message length: " + messageBits.Count);
            int messageBitsIndex = 0;
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex += increment)
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

        protected static byte[] ReadMessageFromBytesLSB(byte[] container, int outBytesCount, int degree = 1, int increment = 1)
        {
            bool[] values = new bool[outBytesCount * 8];
            int messageBitsIndex = 0;
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex += increment)
            {
                if (messageBitsIndex == values.Length)
                    break;
                for (int i = degree - 1; i >= 0; i--)
                {
                    if (messageBitsIndex == values.Length)
                        break;

                    values[messageBitsIndex++] = GetBit(container[containerIndex], 7 - i);
                }
            }
            byte[] result = new byte[outBytesCount];
            BitArray bits = new BitArray(values);
            bits.CopyTo(result, 0);
            return result;
        }

        protected static byte[] ReadAllBytesFrombytesLSB(byte[] container, int degree = 1, int increment = 1)
        {
            List<bool> values = new List<bool>();
            for (int containerIndex = 0; containerIndex < container.Length; containerIndex += increment)
            {
                for (int i = degree - 1; i >= 0; i--)
                {
                    values.Add( GetBit(container[containerIndex], 7 - i) );
                }
            }
            byte[] result = new byte[values.Count / 8];
            BitArray bits = new BitArray(values.ToArray());
            bits.CopyTo(result, 0);
            return result;
        }
    }
}
