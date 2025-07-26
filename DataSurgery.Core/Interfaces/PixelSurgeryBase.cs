using System.Drawing;

namespace DataSurgery.Core.Interfaces;

public abstract class PixelSurgeryBase : SurgeryBase
{
    protected static byte[] BitmapToBytes(Bitmap bitmap)
    {
        var result = new List<byte>();

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                result.Add(bitmap.GetPixel(x, y).R);
                result.Add(bitmap.GetPixel(x, y).G);
                result.Add(bitmap.GetPixel(x, y).B);
            }
        }

        return result.ToArray();
    }

    protected static Bitmap WriteBytesToPixels(byte[] bytes, Bitmap bitmap)
    {
        var q = new Queue<byte>(bytes);
        var result = new Bitmap(bitmap);

        for (int y = 0; y < bitmap.Height; y++)
        {
            for (int x = 0; x < bitmap.Width; x++)
            {
                byte r = q.Dequeue();
                byte g = q.Dequeue();
                byte b = q.Dequeue();
                byte a = bitmap.GetPixel(x, y).A;
                Color color = Color.FromArgb(a, r, g, b);
                result.SetPixel(x, y, color);
            }
        }

        return result;
    }
}
