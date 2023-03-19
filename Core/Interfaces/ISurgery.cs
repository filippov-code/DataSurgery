using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Interfaces
{
    internal interface ISurgery
    {
        int GetFreeSpace();
        byte[] HideWithLSB(byte[] message);
        byte[] FindLSB(int bytesCount);
    }
}
