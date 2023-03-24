﻿using Core.BitmapImage;
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
        public static ISurgery? GetSurgery(string extension, string path, int degree)
        {
            return extension switch
            {
                ".bmp" => new BmpSurgery(path, degree),
                ".gif" => new GifSurgery(path, degree),
                ".jpeg" => new JpegSurgery(path, degree),
                ".jpg" => new JpegSurgery(path, degree),
                ".png" => new PngSurgery(path, degree),
                ".tiff" => new TiffSurgery(path, degree),
                ".wav" => new WavSurgery(path, degree),
                _ => throw new ArgumentException()
            };
        }
    }
}