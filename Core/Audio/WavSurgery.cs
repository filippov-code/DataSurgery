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
        private FileInfo fileInfo;
        private byte[] fileBytes;
        private int startDataIndex;
        public byte[] dataBlock;
        //private readonly int degree;
        //public override int Degree => degree;

        public WavSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);
            //this.degree = degree;

            fileBytes = File.ReadAllBytes(fileInfo.FullName);
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

            BytesForChange = fileBytes[startDataIndex..];
        }

        public override long GetFreeSpace(int degree)
        {
            return dataBlock.Length * degree / 2;
        }

        public override void Save(string path)
        {
            byte[] result = fileBytes[..];
            Buffer.BlockCopy(BytesForChange, 0, result, startDataIndex, BytesForChange.Length);
            File.WriteAllBytes(path, result);
        }

        //public override byte[] HideWithLSB(byte[] message)
        //{
        //    WriteMessageInBytesLSB(dataBlock, message, Degree, 2);
        //    Buffer.BlockCopy(dataBlock, 0, fileBytes, startDataIndex, dataBlock.Length);
        //    //ReplaceElementsInArray(file, dataBlock, startDataIndex);
        //    return fileBytes;
        //}

        //public override byte[] FindLSB(int bytesCount)
        //{
        //    return ReadMessageFromBytesLSB(dataBlock, bytesCount, Degree, 2);
        //}

        //public override byte[] ReadAllBytesLSB()
        //{
        //    return ReadAllBytesFrombytesLSB(dataBlock, Degree, 2);
        //}
    }
}
