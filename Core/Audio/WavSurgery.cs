using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.Audio
{
    public class WavSurgery : SurgeryBase
    {
        private FileInfo fileInfo;
        public readonly int FreeSpace;
        public readonly int Degree;

        public WavSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);

            //FreeSpace = bitmap.Width * bitmap.Height * 3;

            Degree = degree;
        }

        public byte[] HideWithLSB(byte[] message)
        {
            byte[] file = File.ReadAllBytes(fileInfo.FullName);
            int startDataIndex = 0;
            for (int i = 0; i < file.Length - 4; i++)
            {
                if (file[i] == 0x64) //0x64 = 'd' in ANSI
                {
                    if (file[i + 1] == 0x61 && file[i + 2] == 0x74 && file[i + 3] == 0x61)
                    {
                        startDataIndex = i + 4 + 4;
                        break;
                    }

                }
            }
            if (startDataIndex == 0)
                throw new Exception("'data' block not found/");
            byte[] dataBlock = file[startDataIndex..];
            WriteMessageLSB(dataBlock, message, Degree, 2);
            ReplaceElementsInArray(file, dataBlock, startDataIndex);
            return file;
        }

        public byte[] FindLSB(int bitsCount)
        {
            byte[] file = File.ReadAllBytes(fileInfo.FullName);
            int startDataIndex = 0;
            for (int i = 0; i < file.Length - 4; i++)
            {
                if (file[i] == 0x64) //0x64 = 'd' in ANSI
                {
                    if (file[i + 1] == 0x61 && file[i + 2] == 0x74 && file[i + 3] == 0x61)
                    {
                        startDataIndex = i + 4 + 4;
                        break;
                    }

                }
            }
            if (startDataIndex == 0)
                throw new Exception("'data' block not found/");
            byte[] dataBlock = file[startDataIndex..];
            byte[] message = ReadMessageLSB(dataBlock, bitsCount / 8, Degree, 2);
            return message;
        }
    }
}
