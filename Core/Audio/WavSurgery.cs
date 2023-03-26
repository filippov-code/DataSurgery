using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.Audio
{
    public class WavSurgery : BitsSurgeryBase
    {
        private byte[] fileBytes;
        private int startDataIndex;

        public WavSurgery(string path)
        {
            fileBytes = File.ReadAllBytes(path);
            startDataIndex = 0;
            for (int i = 0; i < fileBytes.Length - 4; i++)
            {
                if (fileBytes[i] == 0x64) //0x64 = 'd' in ANSI
                {
                    if (fileBytes[i + 1] == 0x61 && fileBytes[i + 2] == 0x74 && fileBytes[i + 3] == 0x61)
                    {
                        startDataIndex = i + 4 + 4;
                        break;
                    }

                }
            }
            if (startDataIndex == 0)
                throw new Exception("'data' block not found.");

            BytesForChange = fileBytes[startDataIndex..].Select(x => (int)x).ToArray();
        }

        public override void Save(string path)
        {
            byte[] result = fileBytes[..];
            Buffer.BlockCopy(BytesForChange.Select(x => (byte)x).ToArray(), 0, result, startDataIndex, BytesForChange.Length);
            File.WriteAllBytes(path, result);
        }
    }
}
