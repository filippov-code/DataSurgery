using System.Collections;
using System.Text;

namespace DataSurgery.Core;

/// <summary>
/// Data Surgery Meta Data
/// 
/// ABlock:
///     method - 1 byte
///     degree - 1 byte
///     messageSize - 4 bytes
///     hash - 2 bytes
///     extensionSize - 1 byte
/// BBlock:
///     extension
/// ----------------------------
/// Size = ABlock + BBlock = 9 bytes + BBlock
/// </summary>
public class DSMD
{
    public const int Size = 9;
    public static Encoding Encoding = Encoding.UTF8;
    public readonly int ExtensionSize;
    public readonly int SurgeryMethod;
    public readonly int Degree;
    public readonly int MessageSize;
    public readonly byte[] HashTail;

    public DSMD(string extension, int method, int degree, int messageSize, byte[] hash)
    {
        if (extension.Length > byte.MaxValue)
            throw new ArgumentException("The file extension is too large");
        if (degree > 100 || degree < 0)
            throw new ArgumentException("The degree must be between 0 and 100.");
        if (messageSize > Math.Pow(2, 4 * 8) - 1 || messageSize < 0)
            throw new ArgumentException("The size cannot exceed 4GB");

        SurgeryMethod = method;
        Degree = degree;
        MessageSize = messageSize;
        HashTail = hash.TakeLast(2).ToArray();
        ExtensionSize = Encoding.GetBytes(extension).Length;
    }

    public DSMD(byte[] data)
    {
        bool[] bits = new BitArray(data).Cast<bool>().ToArray();
        bool[] mthd = bits[..8];
        bool[] dgre = bits[8..16];
        bool[] mssz = bits[16..48];
        bool[] hstl = bits[48..64];
        bool[] exsz = bits[64..72];

        int method = BitsToInt32(mthd);
        int degree = BitsToInt32(dgre);
        int messageSize = BitsToInt32(mssz);
        byte[] hashTail = BitsToBytes(hstl);
        int extensionSize = BitsToInt32(exsz);

        SurgeryMethod = method;
        Degree = degree;
        MessageSize = messageSize; 
        HashTail = hashTail;
        ExtensionSize = extensionSize;
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

    public byte[] GetABlock()
    {
        bool[] mthd = GetBitsWithInsignificantZeros(SurgeryMethod, 8);
        bool[] dgre = GetBitsWithInsignificantZeros(Degree, 8);
        bool[] mssz = GetBitsWithInsignificantZeros(MessageSize, 4 * 8);
        bool[] hstl = new BitArray(HashTail).Cast<bool>().ToArray();
        bool[] exsz = GetBitsWithInsignificantZeros(ExtensionSize, 8);

        byte[] result = new byte[Size];
        bool[] resultBits = mthd.Concat(dgre).Concat(mssz).Concat(hstl).Concat(exsz).ToArray();
        new BitArray(resultBits).CopyTo(result, 0);

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
