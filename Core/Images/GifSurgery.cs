using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core.BitmapImage
{
    public sealed class GifSurgery : BitsSurgeryBase
    {
        private byte[] fileBytes;

        public GifSurgery(string path)
        {
            fileBytes = File.ReadAllBytes(path);
            int A2 = fileBytes[10];
            BitArray sizeBits = new BitArray
            (
                new bool[]
                {
                    A2 % 2 == 1,
                    (A2 >> 1) % 2 == 1,
                    (A2 >> 2) % 2 == 1
                }
             );
            int[] temp = new int[1];
            sizeBits.CopyTo(temp, 0);
            int PIXELflag = temp[0];
            int paletteSize = (int)Math.Pow(2, PIXELflag + 1);
            BytesForChange = fileBytes[13..(13 + paletteSize * 3)].Select(x => (int)x).ToArray();
        }

        public override void Save(string path)
        {
            byte[] result = fileBytes[..];
            Buffer.BlockCopy(BytesForChange.Select(x => (byte)x).ToArray(), 0, result, 13, BytesForChange.Length);
            File.WriteAllBytes(path, result);
        }
    }
}
