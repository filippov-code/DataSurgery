using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Core.SurgeryBase;

namespace Core
{
    /// <summary>
    /// Data Surgery Meta Data
    /// </summary>
    public class DSMD
    {
        public const int Size = 8;
        public readonly FileFormats Format;
        public readonly SurgeryMethods SurgeryMethod;
        public readonly int Degree;
        public readonly int MessageSize;
        public readonly byte[] HashTail;

        public DSMD(FileFormats format, SurgeryMethods method, int degree, int dataSizeInBytes, byte[] hash)
        {
            if (degree > 100 || degree < 0)
                throw new ArgumentException("The degree must be between 0 and 100.");
            if (dataSizeInBytes > Math.Pow(2, 4 * 8) - 1 || dataSizeInBytes < 0)
                throw new ArgumentException("The size cannot exceed 4GB");
            if (hash.Length < 2)
                throw new ArgumentException("The hash must contain at least 2 bytes");

            Format = format;
            SurgeryMethod = method;
            Degree = degree;
            MessageSize = dataSizeInBytes;
            HashTail = hash.TakeLast(2).ToArray();
        }

        public DSMD(byte[] data)
        {
            bool[] bits = new BitArray(data).Cast<bool>().ToArray();
            bool[] frmt = bits[0..5];
            bool[] mthd = bits[5..9];
            bool[] dgre = bits[9..16];
            bool[] mssz = bits[16..48];
            bool[] hstl = bits[48..64];
            int format = (int)BitsToInt32(frmt);
            int method = (int)BitsToInt32(mthd);
            int degree = (int)BitsToInt32(dgre);
            int messageSize = (int)BitsToInt32(mssz);
            byte[] hashTail = BitsToBytes(hstl);
            Format = (FileFormats)format;
            SurgeryMethod = (SurgeryMethods)method;
            Degree = degree;
            MessageSize = messageSize; 
            HashTail = hashTail;
        }
        //LITTLE ENDIAN CONVERT
        private int BitsToInt32(bool[] bits)
        {
            int[] ints = new int[1];
            new BitArray(bits.Reverse().ToArray()).CopyTo(ints, 0);
            return ints[0];
        }

        private byte[] BitsToBytes(bool[] bits)
        {
            BitArray bitsArray = new BitArray(bits);
            byte[] result = new byte[(int)Math.Ceiling(bitsArray.Length / 8f)];
            bitsArray.CopyTo(result, 0);
            return result;
        }

        public byte[] ToBytes()
        {
            bool[] frmt = GetBitsWithInsignificantZeros((int)Format, 5);
            bool[] mthd = GetBitsWithInsignificantZeros((int)SurgeryMethod, 4);
            bool[] dgre = GetBitsWithInsignificantZeros(Degree, 7);
            bool[] mssz = GetBitsWithInsignificantZeros(MessageSize, 4 * 8);
            bool[] hstl = new BitArray(HashTail).Cast<bool>().ToArray();
            byte[] result = new byte[Size];
            new BitArray(frmt.Concat(mthd).Concat(dgre).Concat(mssz).Concat(hstl).ToArray()).CopyTo(result, 0);
            return result;
        }

        private bool[] GetBitsWithInsignificantZeros(int value, int size)
        {
            string bitsString = Convert.ToString(value, 2);
            if (bitsString.Length > size)
                throw new ArgumentException();
            return new bool[size - bitsString.Length]
                .Concat(bitsString.Select(x => x == '0' ? false : true))
                .ToArray();
        }
    }
}
