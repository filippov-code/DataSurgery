using DataSurgery.Core.Interfaces;
using System.Drawing;

namespace DataSurgery.Core.Images;

public class JpegSurgery : BitsSurgeryBase
{
    private FileInfo fileInfo;
    private Bitmap bitmap;

    public JpegSurgery(string path)
    {
        fileInfo = new FileInfo(path);
        bitmap = new Bitmap(path);
        Init();
    }

    public override void Save(string path)
    {
        Queue<int> bytes = new Queue<int>(BytesForChange);
        BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
        FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        jpegDecompress.jpeg_stdio_src(fs);
        jpegDecompress.jpeg_read_header(true);
        BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
        var YComponent = components[0].Access(0, bitmap.Height / 8);
        for (int y = 0; y < bitmap.Height / 8; y++)
        {
            for (int x = 0; x < bitmap.Width / 8; x++)
            {

                YComponent[y][x][0] = (short)bytes.Dequeue();
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
        List<int> result = new();
        BitMiracle.LibJpeg.Classic.jpeg_decompress_struct jpegDecompress = new BitMiracle.LibJpeg.Classic.jpeg_decompress_struct();
        FileStream fs = new FileStream(fileInfo.FullName, FileMode.Open, FileAccess.Read);
        jpegDecompress.jpeg_stdio_src(fs);
        jpegDecompress.jpeg_read_header(true);
        BitMiracle.LibJpeg.Classic.jvirt_array<BitMiracle.LibJpeg.Classic.JBLOCK>[] components = jpegDecompress.jpeg_read_coefficients();
        var YComponent = components[0].Access(0, bitmap.Height / 8);
        for (int y = 0; y < bitmap.Height / 8; y++)
        {
            for (int x = 0; x < bitmap.Width / 8; x++)
            {
                result.Add(YComponent[y][x][0]);
            }
        }

        //Jblock[индекс компоненты (0-2)].Access(с какой строки начать, сколько строк 8х8 взять)
        //ll[строка из блоков 8х8][блок 8х8 по порядку][пиксель в блоке 8х8 по порядку]

        jpegDecompress.jpeg_finish_decompress();
        fs.Close();

        BytesForChange = result.ToArray();
    }
}
