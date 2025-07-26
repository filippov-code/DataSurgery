using Core.BitmapImage;
using Core.Audio;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Core.Interfaces;

namespace Core
{
    public static class SurgeryFactory
    {
        public static ISurgery? GetSurgery(string path)
        {
            string extension = Path.GetExtension(path);
            return extension switch
            {
                ".bmp" => new BmpSurgery(path),
                ".gif" => new GifSurgery(path),
                ".jpeg" => new JpegSurgery(path),
                ".jpg" => new JpegSurgery(path),
                ".png" => new PngSurgery(path),
                ".tiff" => new TiffSurgery(path),
                ".wav" => new WavSurgery(path),
                _ => null
            };
        }
    }
}
