using Core.Interfaces;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Core.BitmapImage
{
    public class JpegSurgery : BitsSurgeryBase
    {
        private FileInfo fileInfo;
        private Bitmap bitmap;
        //private int degree;
        //public override int Degree => degree;

        public JpegSurgery(string path, int degree = 1)
        {
            fileInfo = new FileInfo(path);
            bitmap = new Bitmap(path);
            Init();
            //this.degree = degree;
        }

        public override long GetFreeSpace(int degree)
        {
            return (bitmap.Height/8) * (bitmap.Width/8) * degree;
        }

        public override void Save(string path)
        {
            Queue<byte> bytes = new Queue<byte>(BytesForChange);
            BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            jpegDecompress.jpeg_stdio_src(fs);
            jpegDecompress.jpeg_read_header(true);
            BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
            var YComponent = components[0].Access(0, bitmap.Height / 8);
            //int bitsCount = bytesCount * 8;
            //bool[] messageValues = new bool[bitsCount];
            int bitsIndex = 0;
            for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
            {
                //if (bitsIndex == bitsCount)
                //    break;

                for (int x = 0; x < bitmap.Width / 8; x++)
                {
                    //if (bitsIndex == bitsCount)
                    //    break;

                    //Console.WriteLine($"value: {YComponent[y][x][0]} lbit: {GetBit((byte)YComponent[y][x][0], 7)}");
                    YComponent[y][x][0] = bytes.Dequeue();
                }
            }
            //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
            //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
            jpegDecompress.jpeg_finish_decompress();
            fs.Close();

            FileStream fss = File.Create(path);
            BitMiracle.LibJpeg.Classic.jpeg_compress_struct oJpegCompress = new BitMiracle.LibJpeg.Classic.jpeg_compress_struct();
            oJpegCompress.jpeg_stdio_dest(fss);
            jpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
            oJpegCompress.Image_height = bitmap.Height;
            oJpegCompress.Image_width = bitmap.Width;
            oJpegCompress.jpeg_write_coefficients(components);
            oJpegCompress.jpeg_finish_compress();
            fs.Close();
            jpegDecompress.jpeg_abort_decompress();
            fss.Close();
        }

        private void Init()
        {
            //Console.WriteLine("||||| FindLSB |||||");
            List<byte> result = new();
            BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
            FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
            jpegDecompress.jpeg_stdio_src(fs);
            jpegDecompress.jpeg_read_header(true);
            BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
            var YComponent = components[0].Access(0, bitmap.Height / 8);
            //int bitsCount = bytesCount * 8;
            //bool[] messageValues = new bool[bitsCount];
            int bitsIndex = 0;
            for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
            {
                //if (bitsIndex == bitsCount)
                //    break;

                for (int x = 0; x < bitmap.Width / 8; x++)
                {
                    //if (bitsIndex == bitsCount)
                    //    break;

                    //Console.WriteLine($"value: {YComponent[y][x][0]} lbit: {GetBit((byte)YComponent[y][x][0], 7)}");
                    result.Add((byte)YComponent[y][x][0]);
                }
            }
            //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
            //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
            jpegDecompress.jpeg_finish_decompress();
            fs.Close();

            BytesForChange =  result.ToArray();
        }

        //public override byte[] HideWithLSB(byte[] message)
        //{
        //    //Console.WriteLine("||||| HideWithLSB |||||");
        //    BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
        //    FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        //    jpegDecompress.jpeg_stdio_src(fs);
        //    jpegDecompress.jpeg_read_header(true);
        //    BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
        //    var YComponent = components[0].Access(0, bitmap.Height / 8);
        //    BitArray messageBits = new BitArray(message);
        //    int bitsIndex = 0;
        //    for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
        //    {
        //        if (bitsIndex == messageBits.Count)
        //            break;

        //        for (int x = 0; x < bitmap.Width / 8; x++)
        //        {
        //            if (bitsIndex == messageBits.Count)
        //                break;

        //            short newValue = Convert.ToInt16(SetBit((byte)YComponent[y][x][0], 7, messageBits[bitsIndex++]));
        //            //Console.WriteLine($"{YComponent[y][x][0]}-{messageBits[bitsIndex-1]}-{newValue}");
        //            //if (Math.Abs(newValue - YComponent[y][x][0]) > 1) Console.WriteLine("caramba");
        //            YComponent[y][x][0] = newValue;
        //            //if (bitsIndex < 20) Console.Write(YComponent[y][x][0] + ", ");
        //        }
        //    }
        //    //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
        //    //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
        //    jpegDecompress.jpeg_finish_decompress();
        //    fs.Close();
        //    //return null;
        //    /////
        //    //FileStream ms = File.Create("test_dct.jpg");
        //    MemoryStream ms = new MemoryStream();
        //    BitMiracle.LibJpeg.Classic.jpeg_compress_struct oJpegCompress = new BitMiracle.LibJpeg.Classic.jpeg_compress_struct();
        //    oJpegCompress.jpeg_stdio_dest(ms);
        //    jpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
        //    oJpegCompress.Image_height = bitmap.Height;
        //    oJpegCompress.Image_width = bitmap.Width;
        //    oJpegCompress.jpeg_write_coefficients(components);
        //    oJpegCompress.jpeg_finish_compress();
        //    ms.Close();
        //    jpegDecompress.jpeg_abort_decompress();
        //    fs.Close();
        //    //Console.WriteLine();
        //    return ms.ToArray();
        //}

        //public override byte[] FindLSB(int bytesCount)
        //{
        //    //Console.WriteLine("||||| FindLSB |||||");
        //    BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
        //    FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        //    jpegDecompress.jpeg_stdio_src(fs);
        //    jpegDecompress.jpeg_read_header(true);
        //    BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
        //    var YComponent = components[0].Access(0, bitmap.Height / 8);
        //    int bitsCount = bytesCount * 8;
        //    bool[] messageValues = new bool[bitsCount];
        //    int bitsIndex = 0;
        //    for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
        //    {
        //        if (bitsIndex == bitsCount)
        //            break;

        //        for (int x = 0; x < bitmap.Width / 8; x++)
        //        {
        //            if (bitsIndex == bitsCount)
        //                break;

        //            //Console.WriteLine($"value: {YComponent[y][x][0]} lbit: {GetBit((byte)YComponent[y][x][0], 7)}");
        //            messageValues[bitsIndex++] = GetBit((byte)YComponent[y][x][0], 7);
        //        }
        //    }
        //    //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
        //    //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
        //    jpegDecompress.jpeg_finish_decompress();
        //    fs.Close();

        //    BitArray messageBits = new BitArray(messageValues);
        //    if (bitsIndex != messageBits.Count) Console.WriteLine("caramba");
        //    byte[] bytes = new byte[bytesCount];
        //    messageBits.CopyTo(bytes, 0);
        //    //Console.WriteLine();
        //    return bytes;
        //}

        //public override byte[] ReadAllBytesLSB()
        //{
        //    //Console.WriteLine("||||| FindLSB |||||");
        //    BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
        //    FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        //    jpegDecompress.jpeg_stdio_src(fs);
        //    jpegDecompress.jpeg_read_header(true);
        //    BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
        //    var YComponent = components[0].Access(0, bitmap.Height / 8);
        //    List<bool> messageValues = new List<bool>();
        //    for (int y = 0; y < bitmap.Height / 8; y++) // some cycle
        //    {
        //        for (int x = 0; x < bitmap.Width / 8; x++)
        //        {
        //            //Console.WriteLine($"value: {YComponent[y][x][0]} lbit: {GetBit((byte)YComponent[y][x][0], 7)}");
        //            messageValues.Add(GetBit((byte)YComponent[y][x][0], 7));
        //        }
        //    }
        //    //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
        //    //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
        //    jpegDecompress.jpeg_finish_decompress();
        //    fs.Close();

        //    BitArray messageBits = new BitArray(messageValues.ToArray());
        //    //if (bitsIndex != messageBits.Count) Console.WriteLine("caramba");
        //    byte[] bytes = new byte[messageBits.Count / 8];
        //    messageBits.CopyTo(bytes, 0);
        //    //Console.WriteLine();
        //    return bytes;
        //}
    }
}
