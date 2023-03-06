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
        private Bitmap bitmap;
        public readonly int FreeSpace;
        public readonly int Degree;

        public JpegSurgery(string path, int degree = 1)
        {
            bitmap = new Bitmap(path);

            FreeSpace = bitmap.Width * bitmap.Height * 3;

            Degree = degree;
        }

        public byte[] HideWithLSB(byte[] message)
        {
            string path = @"test.jpg";
            var img = new Bitmap(path);
            var jo = img.Width;
            var joj = img.Height;
            BitMiracle.LibJpeg.Classic.jpeg_decompress_struct oJpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
            FileStream oFileStreamImage = new FileStream(path, FileMode.Open, FileAccess.Read);
            oJpegDecompress.jpeg_stdio_src(oFileStreamImage);
            oJpegDecompress.jpeg_read_header(true);
            BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] JBlock = oJpegDecompress.jpeg_read_coefficients();
            var ll = JBlock[0].Access(0, 1); // accessing the element
            Console.WriteLine(JBlock.Length);
            var oo = 5; // its gonna be new value for coefficient
            for (int i = 0; i < 64; i++) // some cycle
            {
                ll[0][i][63] = Convert.ToInt16(oo); // changes
                //Jblock[].Access(с какой строки начать, сколько строк 8х8 взять)
                //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]
            }
            oJpegDecompress.jpeg_finish_decompress();
            oFileStreamImage.Close();
            return null;
            /////
            FileStream objFileStreamMegaMap = File.Create("test_dct.jpg");
            BitMiracle.LibJpeg.Classic.jpeg_compress_struct oJpegCompress = new BitMiracle.LibJpeg.Classic.jpeg_compress_struct();
            oJpegCompress.jpeg_stdio_dest(objFileStreamMegaMap);
            oJpegDecompress.jpeg_copy_critical_parameters(oJpegCompress);
            oJpegCompress.Image_height = joj;
            oJpegCompress.Image_width = jo;
            oJpegCompress.jpeg_write_coefficients(JBlock);
            oJpegCompress.jpeg_finish_compress();
            objFileStreamMegaMap.Close();
            oJpegDecompress.jpeg_abort_decompress();
            oFileStreamImage.Close();
            return null;
        }

        private ImageCodecInfo GetEncoder(ImageFormat format)
        {
            ImageCodecInfo[] codecs = ImageCodecInfo.GetImageDecoders();
            foreach (ImageCodecInfo codec in codecs)
            {
                if (codec.FormatID == format.Guid)
                {
                    return codec;
                }
            }
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
            Console.WriteLine(@"||||| FindLSB |||||");
            int bitsIndex = 0;
            bool[] messageValues = new bool[bitsCount];
            for (int y = 0; y < bitmap.Height; y++)
            {
                if (bitsIndex == bitsCount)
                    break;

                for (int x = 0; x < bitmap.Width; x++)
                {
                    if (bitsIndex == bitsCount)
                        break;

                    Color pixel = bitmap.GetPixel(x, y);
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.R, 7 - i);
                    }
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.G, 7 - i);
                    }
                    for (int i = Degree - 1; i >= 0; i--)
                    {
                        if (bitsIndex == bitsCount)
                            break;

                        messageValues[bitsIndex++] = GetBit(pixel.B, 7 - i);
                    }
                }
            }
            BitArray bits = new BitArray(messageValues);
            Console.WriteLine("bits finded: " + bits.Count);
            byte[] bytes = new byte[bits.Length / 8];
            bits.CopyTo(bytes, 0);
            Console.WriteLine("bytes finded: " + bytes.Length);
            return bytes;
        }

        public static bool GetBit(int value, int index)
        {
            value >>= (7 - index);
            return value % 2 == 1;
        }
    }
}
