using System.Security.Cryptography;
using System.Text;
using DataSurgery.Encryption.Interfaces;

namespace DataSurgery.Encryption;

public class AES : ICryptoProvider
{
    private ICryptoTransform encryptor;
    private ICryptoTransform decryptor;


    public AES(string key, string iv) 
        : this(Encoding.UTF8.GetBytes(key), Encoding.UTF8.GetBytes(iv))
    {

    }

    public AES(byte[] key, byte[] iv)
    {
        var aes = Aes.Create();

        aes.Key = SHA256.Create().ComputeHash(key);
        aes.IV = MD5.Create().ComputeHash(iv);
        aes.Mode = CipherMode.CBC;
        aes.Padding = PaddingMode.PKCS7;

        encryptor = aes.CreateEncryptor(aes.Key, aes.IV);
        decryptor = aes.CreateDecryptor(aes.Key, aes.IV);
    }

    public byte[] Encrypt(params byte[] plainText)
    {
        return encryptor.TransformFinalBlock(plainText, 0, plainText.Length);
    }

    public byte[] Decrypt(params byte[] ciphre)
    {
        return decryptor.TransformFinalBlock(ciphre, 0, ciphre.Length);
    }
}