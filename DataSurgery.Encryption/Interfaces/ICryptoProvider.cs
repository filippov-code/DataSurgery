namespace DataSurgery.Encryption.Interfaces;

public interface ICryptoProvider
{
    public byte[] Encrypt(params byte[] message);

    public byte[] Decrypt(params byte[] ciphre);
}
