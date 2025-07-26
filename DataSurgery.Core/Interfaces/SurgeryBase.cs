using System.Collections;

namespace DataSurgery.Core.Interfaces;

public abstract class SurgeryBase : ISurgery
{
    public int[] BytesForChange { get; set; }

    protected int changeIndex = 0;

    public virtual long GetFreeSpace(int degree)
    {
        return (BytesForChange.Length - changeIndex) * degree / 8;
    }

    public void AddLSB(byte[] message, int degree)
    {
        var messageBits = new BitArray(message);

        for (int i = 0; i < messageBits.Count / degree; i++)
        {
            if (changeIndex == BytesForChange.Length)
                throw new ArgumentException("Space not enought");

            for (int j = 0; j < degree; j++)
            {
                BytesForChange[changeIndex] = SetBit(BytesForChange[changeIndex], j, messageBits[i * degree + j]);
            }
            changeIndex++;
        }
    }

    public byte[] FindLSB(int bytesCount, int degree, int startIndex)
    {
        var values = new bool[bytesCount * 8];
        int bitsIndex = 0;
        for (int i = startIndex; i < BytesForChange.Length; i++)
        {
            if (bitsIndex == values.Length)
                break;

            for (int j = 0; j < degree; j++)
            {
                if (bitsIndex == values.Length)
                    break;

                values[bitsIndex++] = GetBit(BytesForChange[i], j);
            }
        }

        var result = new byte[bytesCount];
        var bits = new BitArray(values);
        bits.CopyTo(result, 0);

        return result;
    }

    public abstract void Save(string path);

    public static int SetBit(int value, int index, bool bitValue)
    {
        int result = value;
        int mask = 1 << index;

        if (bitValue)
            result |= mask;
        else
            result &= ~mask;

        return result;
    }

    public static bool GetBit(int value, int index)
    {
        value >>= index;

        return Math.Abs(value % 2) == 1;
    }
}
