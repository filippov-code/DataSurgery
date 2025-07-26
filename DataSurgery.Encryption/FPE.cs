using DataSurgery.Encryption.Interfaces;

namespace DataSurgery.Encryption;

public class FPE
{
    private ICryptoProvider cryptoProvider;

    public FPE(ICryptoProvider provider)
    {
        cryptoProvider = provider;
    }

    public T[] PrefixEncrypt<T>(T[] plaintext)
    {
        var indexesAndWeights = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < plaintext.Length; i++)
        {
            indexesAndWeights.Add( new KeyValuePair<int, int>(i, BitConverter.ToInt32(cryptoProvider.Encrypt((byte)i))) );
        }

        var sortedIndexesAndWeights = indexesAndWeights.OrderBy(x => x.Value).ToList();
        var result = new T[plaintext.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[i] = plaintext[sortedIndexesAndWeights[i].Key];
        }

        return result;
    }

    public byte[] PrefixDecrypt(byte[] ciphre)
    {
        var indexesAndWeights = new List<KeyValuePair<int, int>>();
        for (int i = 0; i < ciphre.Length; i++)
        {
            indexesAndWeights.Add(new KeyValuePair<int, int>(i, BitConverter.ToInt32(cryptoProvider.Encrypt((byte)i))));
        }
        var sortedIndexesAndWeights = indexesAndWeights.OrderBy(x => x.Value).ToList();

        var result = new byte[ciphre.Length];
        for (int i = 0; i < result.Length; i++)
        {
            result[sortedIndexesAndWeights[i].Key] = ciphre[i];
        }

        return result;
    }
}
