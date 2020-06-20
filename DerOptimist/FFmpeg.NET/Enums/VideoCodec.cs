using Newtonsoft.Json;
using System;
using System.Collections.ObjectModel;

namespace FFmpeg.NET.Enums
{
    public static class VideoCodec
    {
        public static VideoCodecCollection SettingsCollection;

        public static string H264 { get { return "H264"; } }
        public static string H264_constant { get { return "H264_constant"; } }
        public static string H265 { get { return "H265"; } }
        public static string H264_nvenc { get { return "H264_nvenc"; } }
        public static string HEVC_nvenc { get { return "HEVC_nvenc"; } }
        public static string ProRes422_Proxy { get { return "ProRes422_Proxy"; } }
        public static string ProRes422_LT { get { return "ProRes422_LT"; } }
        public static string ProRes422_Normal { get { return "ProRes422_Normal"; } }
        public static string ProRes422_HQ { get { return "ProRes422_HQ"; } }
        public static string ProRes4444 { get { return "ProRes4444"; } }
        public static string ProRes4444Alpha { get { return "ProRes4444Alpha"; } }
        public static string DNxHD { get { return "DNxHD"; } }
        public static string DNxHR_444 { get { return "DNxHR_444"; } }
        public static string DNxHR_HQX { get { return "DNxHR_HQX"; } }
        public static string DNxHR_HQ { get { return "DNxHR_HQ"; } }
        public static string DNxHR_SQ { get { return "DNxHR_SQ"; } }
        public static string DNxHR_LB { get { return "DNxHR_LB"; } }
        public static string Hap { get { return "Hap"; } }
        public static string Hap_Alpha { get { return "Hap_Alpha"; } }
        public static string HapQ { get { return "HapQ"; } }
        public static string PNG { get { return "PNG"; } }
        public static string Jpeg2000 { get { return "Jpeg2000"; } }
        public static string Theora { get { return "Theora"; } }
        public static string VP8 { get { return "VP8"; } }
        public static string VP9 { get { return "VP9"; } }
        public static string Copy { get { return "Copy"; } }
        public static string None { get { return "None"; } }
        public static string Default = H264;

        static VideoCodec() {
            SettingsCollection = new VideoCodecCollection();
            SettingsCollection.Add(new VideoCodecEntry("Copy") { Encoder = "copy", FileExtension = null, QualityMode = null });
            SettingsCollection.Add(new VideoCodecEntry("None") { Encoder = "None", FileExtension = null, QualityMode = null });
            SettingsCollection.Add(new VideoCodecEntry("H264") {Encoder = "libx264", FileExtension = ".mp4", OutputArgs = "-movflags +faststart", PixFmt = "yuv420p", QualityMode = "crf", QualityMin=0, QualityMax=51, QualityDefault=18});
            SettingsCollection.Add(new VideoCodecEntry("H265") { Encoder = "libx265", FileExtension = ".mp4", OutputArgs = "-movflags +faststart", PixFmt = "yuv420p", QualityMode = "crf", QualityMin = 0, QualityMax = 51, QualityDefault = 18 });
            SettingsCollection.Add(new VideoCodecEntry("H264_nvenc") { Encoder = "h264_nvenc", OutputArgs = "-movflags +faststart -preset llhq -rc constqp -strict experimental", PixFmt = "yuv420p", FileExtension = ".mp4", QualityMode = "qp", QualityMin = 0, QualityMax = 51, QualityDefault = 18 });
            SettingsCollection.Add(new VideoCodecEntry("HEVC_nvenc") { Encoder = "hevc_nvenc", OutputArgs = "-movflags +faststart -preset llhq -rc constqp -strict experimental", PixFmt = "yuv420p", FileExtension = ".mp4", QualityMode = "qp", QualityMin = 0, QualityMax = 51, QualityDefault = 18 });
            SettingsCollection.Add(new VideoCodecEntry("HEVC_nvenc main10") { Encoder = "hevc_nvenc", OutputArgs = "-movflags +faststart -preset slow -profile main10 -level 5.1 -tier high -rc constqp -gpu any -strict experimental -x265-params colorprim=bt2020:transfer=smpte-st-2084:colormatrix=bt2020ncl", PixFmt = "p010le", FileExtension = ".mp4", QualityMode = "qp", QualityMin = 0, QualityMax = 51, QualityDefault = 18 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes422_Proxy") { Encoder = "prores_ks", OutputArgs= "-profile:v 0", PixFmt = "yuv422p10", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes422_LT") { Encoder = "prores_ks", OutputArgs = "-profile:v 1", PixFmt = "yuv422p10", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes422_Normal") { Encoder = "prores_ks", OutputArgs = "-profile:v 2", PixFmt = "yuv422p10", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes422_HQ") { Encoder = "prores_ks", OutputArgs = "-profile:v 3", PixFmt = "yuv422p10", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes4444") { Encoder = "prores_ks", OutputArgs = "-profile:v 4", PixFmt = "yuv444p10", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("ProRes4444Alpha") { Encoder = "prores_ks", OutputArgs = "-profile:v 4 -alpha_bits 16", PixFmt = "yuva444p10le", FileExtension = ".mov", QualityMode = "qscale", QualityMin = 0, QualityMax = 32, QualityDefault = 9 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHD") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhd ", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix="M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHR_444") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhr_444", PixFmt = "yuv422p10le", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix = "M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHR_HQX") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhr_hqx", PixFmt = "yuv422p10le", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix = "M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHR_HQ") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhr_hq ", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix = "M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHR_SQ") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhr_sq ", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix = "M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("DNxHR_LB") { Encoder = "dnxhd", OutputArgs = "-profile:v dnxhr_lb ", FileExtension = ".mxf", QualityMode = "b:v", QualityPostfix = "M", QualityMin = 1, QualityMax = 440, QualityDefault = 145 });
            SettingsCollection.Add(new VideoCodecEntry("Hap") { Encoder = "hap", OutputArgs = "-format hap", FileExtension = ".mov", QualityMode = "chunks", QualityMin = 1, QualityMax = 64, QualityDefault = 1 });
            SettingsCollection.Add(new VideoCodecEntry("Hap_Alpha") { Encoder = "hap", OutputArgs = "-format hap_alpha", FileExtension = ".mov", QualityMode = "chunks", QualityMin = 1, QualityMax = 64, QualityDefault = 1 });
            SettingsCollection.Add(new VideoCodecEntry("HapQ") { Encoder = "hap", OutputArgs = "-format hap_q", FileExtension = ".mov", QualityMode = "chunks", QualityMin = 1, QualityMax = 64, QualityDefault = 1 });
            //SettingsCollection.Add(new VideoCodecEntry("PNG") { Encoder = "png", FileExtension = ".png" });
            //SettingsCollection.Add(new VideoCodecEntry("Jpeg2000") { Encoder = "jpeg2000", OutputArgs = "-format jp2", FileExtension = ".jp2", QualityMode = "q", QualityMin = 1, QualityMax = 31, QualityDefault = 5 });
            SettingsCollection.Add(new VideoCodecEntry("H264_constant") { Encoder = "libx264", FileExtension = ".mp4", OutputArgs = "-x264opts nal-hrd=cbr:force-cfr=1", PixFmt= "yuv420p", QualityMode = "b:v", QualityMin = 100, QualityMax = 32000, QualityDefault = 1500, QualityStep = 100, QualityPostfix="k", MinBitRate=0, MaxBitRate=0, BufferBitRate=-1 });
            SettingsCollection.Add(new VideoCodecEntry("Theora") { Encoder = "libtheora", FileExtension = ".ogv", PixFmt = "yuv420p", QualityMode = "q", QualityMin = 0, QualityMax = 10, QualityDefault = 5 });
            SettingsCollection.Add(new VideoCodecEntry("VP8") { Encoder = "libvpx", FileExtension = ".webm", OutputArgs = "-b:v 0", PixFmt = "yuv420p", QualityMode = "crf", QualityMin = 0, QualityMax = 63, QualityDefault = 25 });
            SettingsCollection.Add(new VideoCodecEntry("VP9") { Encoder = "libvpx-vp9", FileExtension = ".webm", OutputArgs = "-b:v 0", PixFmt = "yuv420p", QualityMode = "crf", QualityMin = 0, QualityMax = 63, QualityDefault = 25 });

        }

        public static VideoCodecEntry Settings(string codecName)
        {
            if (SettingsCollection.Contains(codecName))
            {
                return SettingsCollection[codecName];
            }
            else
            {
                return SettingsCollection[Default];
            }
        }
    }

    
    public class VideoCodecCollection : KeyedCollection<string, VideoCodecEntry>
    {
        protected override string GetKeyForItem(VideoCodecEntry item)
        {
            // In this example, the key is the part number.
            return item.Name;
        }
    }

    [Serializable]
    public class VideoCodecEntry
    {
        /// <summary>
        /// A unique name given to this entry. This is used as the collection Key value.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Encoder library used to encode preview file
        /// </summary>
        public string Encoder { get; set; }

        /// <summary>
        /// Additional codec specific arguments to be added to the output arguments
        /// </summary>
        public string OutputArgs { get; set; } = "";

        /// <summary>
        /// Default file extension
        /// </summary>
        public string FileExtension { get; set; } = null;

        public string QualityMode { get; set; } = null;
        public int QualityMin { get; set; } = 0;
        public int QualityMax { get; set; } = 51;
        public int QualityDefault { get; set; } = 18;
        public int QualityStep { get; set; } = 1;
        public string QualityPostfix { get; set; } = "";
        public int? MinBitRate { get; set; } = null;
        public int? MaxBitRate { get; set; } = null;
        public int? BufferBitRate { get; set; } = null;
        public string PixFmt { get; set; } = "";
        /// <summary>
        /// Summary of various codec specific settings to be used with ffmpeg.
        /// </summary>
        /// <param name="Name">A unique name given to this entry. This is used as the collection Key value.</param>
        [JsonConstructor]
        public VideoCodecEntry(string Name)
        {
            this.Name = Name;
        }

        public override string ToString()
        {
            return Name;
        }
    }
}
