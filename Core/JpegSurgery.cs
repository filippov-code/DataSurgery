using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core
{
    public class JpegSurgery
    {
        private FileInfo fileInfo;
        private Bitmap bitmap;
        public readonly int FreeSpace;
        public readonly int Degree;

        public List<short> tmpBeforeSave = new();
        public List<short> tmpAfterSave = new();

        public JpegSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);
            bitmap = new Bitmap(path);

            FreeSpace = bitmap.Width * bitmap.Height * 3;

            Degree = degree;
        }

        public byte[] HideWithLSB(byte[] message)
        {
            Console.WriteLine("||||| HideWithLSB |||||");
            BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            jpegDecompress.jpeg_stdio_src(fs);
            jpegDecompress.jpeg_read_header(true);
            BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
            var YComponent = components[0].Access(0, bitmap.Height/8);
            BitArray messageBits = new BitArray(message);
            int bitsIndex = 0;
            for (int y = 0; y < bitmap.Height/8; y++) // some cycle
            {
                if (bitsIndex == messageBits.Count)
                    break;

                for (int x = 0; x < bitmap.Width/8; x++)
                {
                    if (bitsIndex == messageBits.Count)
                        break;

                    short newValue = Convert.ToInt16(SetBit(YComponent[y][x][0], 7, messageBits[bitsIndex++]));
                    //Console.WriteLine($"{YComponent[y][x][0]}-{messageBits[bitsIndex-1]}-{newValue}");
                    //if (Math.Abs(newValue - YComponent[y][x][0]) > 1) Console.WriteLine("caramba");
                    tmpBeforeSave.Add(newValue);
                    YComponent[y][x][0] = newValue;
                    //if (bitsIndex < 20) Console.Write(YComponent[y][x][0] + ", ");
                }
            }
            //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
            //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
            jpegDecompress.jpeg_finish_decompress();
            fs.Close();
            //return null;
            /////
            FileStream objFileStreamMegaMap = File.Create("test_dct.jpg");
            BitMiracle.LibJpeg.Classic.jpeg_compress_struct oJpegCompress = new BitMiracle.LibJpeg.Classic.jpeg_compress_struct();
            oJpegCompress.jpeg_stdio_dest(objFileStreamMegaMap);
            jpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
            oJpegCompress.Image_height = bitmap.Height;
            oJpegCompress.Image_width = bitmap.Width;
            oJpegCompress.jpeg_write_coefficients(components);
            oJpegCompress.jpeg_finish_compress();
            objFileStreamMegaMap.Close();
            jpegDecompress.jpeg_abort_decompress();
            fs.Close();
            Console.WriteLine();
            return null;
        }

        private int SetBit(int value, int index, bool bit)
        {
            int tempMask = 1 << (7 - index);
            int result = value & ~tempMask;
            if (bit)
                result |= tempMask;
            return result;
        }

        public byte[] FindLSB(int bitsCount)
        {
            Console.WriteLine("||||| FindLSB |||||");
            BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            jpegDecompress.jpeg_stdio_src(fs);
            jpegDecompress.jpeg_read_header(true);
            BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
            var YComponent = components[0].Access(0, bitmap.Height / 8);
            bool[] messageValues = new bool[bitsCount];
            int bitsIndex = 0;
            for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
            {
                if (bitsIndex == bitsCount)
                    break;

                for (int x = 0; x < bitmap.Width / 8; x++)
                {
                    if (bitsIndex == bitsCount)
                        break;

                    Console.WriteLine($"value: {YComponent[y][x][0]} lbit: {GetBit(YComponent[y][x][0], 7)}");
                    messageValues[bitsIndex++] = GetBit(YComponent[y][x][0], 7);
                }
            }
            //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
            //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
            jpegDecompress.jpeg_finish_decompress();
            fs.Close();

            BitArray messageBits = new BitArray(messageValues);
            if (bitsIndex != messageBits.Count) Console.WriteLine("suka");
            byte[] bytes = new byte[bitsCount / 8];
            messageBits.CopyTo(bytes, 0);
            Console.WriteLine();
            return bytes;
        }

        public static bool GetBit(int value, int index)
        {
            value >>= (7 - index);
            return Math.Abs(value % 2) == 1;
        }
    }
}
