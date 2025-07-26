using DataSurgery.Core.Interfaces;
using System.Drawing;
using System.Drawing.Imaging;

namespace DataSurgery.Core.Images;

public sealed class BmpSurgery : PixelSurgeryBase
{
    private Bitmap bitmap;

    public BmpSurgery(string path)
    {
        bitmap = new Bitmap(path);

        BytesForChange = BitmapToBytes(bitmap).Select(x => (int)x).ToArray();
    }

    public override void Save(string path)
    {
        var result = WriteBytesToPixels(BytesForChange.Select(x => (byte)x).ToArray(), bitmap);

        result.Save(path, ImageFormat.Bmp);
    }
}