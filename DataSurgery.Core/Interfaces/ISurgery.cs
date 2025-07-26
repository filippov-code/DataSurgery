namespace DataSurgery.Core.Interfaces;

public interface ISurgery
{
    long GetFreeSpace(int degree);

    void AddLSB(byte[] message, int degree);

    byte[] FindLSB(int bytesCount, int degree, int startIdnex);

    void Save(string path);
}
