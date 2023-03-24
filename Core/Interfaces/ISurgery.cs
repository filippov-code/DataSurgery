using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    public interface ISurgery
    {
        long GetFreeSpace(int degree);
        void AddWithLSB(byte[] message, int degree);
        byte[] FindLSB(int bytesCount, int degree, int startIdnex);
        void Save(string path);

        //byte[] HideWithLSB(byte[] message);
        //byte[] FindLSB(int bytesCount);
        //byte[] ReadAllBytesLSB();
    }
}
