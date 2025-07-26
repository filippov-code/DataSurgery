using DataSurgery.Core.Audio;
using DataSurgery.Core.Images;
using DataSurgery.Core.Interfaces;

namespace DataSurgery.Domain;

public static class SurgeryFactory
{
    public static ISurgery? GetSurgery(string path)
    {
        var extension = Path.GetExtension(path);

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
