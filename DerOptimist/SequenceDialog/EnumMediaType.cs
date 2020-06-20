using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DerOptimist
{
    //public enum ExtensionsImages
    //{
    //    jpg, png, tif, tiff, dpx, exr, dds, tga, iff, bmp, gif, img, jpe, jpeg, pbm, pcx, pct, ppm, ras, psd, psb, yuv
    //}

    //public enum ExtensionsMovies
    //{
    //    avi, mp4, mpg, mov, mxf, mkv, ogm, flv, m4v, rm, wmv
    //}

    //public enum ExtensionsAudio
    //{
    //    mp3, aiff, aif, m4a, mpa, wma, aac, wav, ogg
    //}

    //public enum ExtensionsSkip
    //{
    //    preset, com, exe, txt, doc, xls, xlss, js, cpp, DS_Store, rar, zip
    //}

    public enum MediaType
    {
        Movie,
        Audio,
        Image,
        ImageSequence,
        Directory,
        Drive,
        Location,
        Bookmark,
        Up,
        All,
        Unknown
    }
}
