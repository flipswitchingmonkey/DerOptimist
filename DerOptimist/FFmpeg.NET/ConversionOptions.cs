using System;
using System.Collections.Generic;
using FFmpeg.NET.Enums;

namespace FFmpeg.NET
{
    public class ConversionOptions
    {   
        /// <summary>
        ///     Specify video encoder specific settings from the list of VideoCodec enums (or define your own VideoCodecEntry)
        /// </summary>
        public VideoCodecEntry VideoEncoder { get; set; } = VideoCodec.Settings(VideoCodec.Default);

        /// <summary>
        ///     Specify video encoder specific settings from the list of VideoCodec enums (or define your own VideoCodecEntry)
        /// </summary>
        public AudioCodecEntry AudioEncoder { get; set; } = AudioCodec.Settings(AudioCodec.Default);

        /// <summary>
        ///     Second input file, e.g. a replacement audio track
        /// </summary>
        public MediaFile SecondInput { get; set; }

        /// <summary>
        ///     Arguments to be inserted immediately after ffmpeg, before first input
        /// </summary>
        public string PreArgs { get; set; } = null;

        /// <summary>
        ///     Explicitely sets the timecode of the first frame
        /// </summary>
        public string TimeCode { get; set; } = null;

        /// <summary>
        ///     Map channels
        /// </summary>
        public List<ChannelMapping> Mappings { get; set; } = new List<ChannelMapping>();

        /// <summary>
        ///     Video Filters
        /// </summary>
        public List<string> VideoFilters { get; set; } = new List<string>();

        /// <summary>
        ///     Pixel Format
        /// </summary>
        public string PixFmt { get; set; } = "";

        /// <summary>
        ///     Input frame rate
        /// </summary>
        public string InputFps { get; set; } = null;

        public int QualityVideo { get; set; } = 20;
        public int QualityAudio { get; set; } = 20;
        /// <summary>
        ///     Audio bit rate
        /// </summary>
        public int? AudioBitRate { get; set; } = null;

        /// <summary>
        ///     Audio sample rate
        /// </summary>
        public AudioSampleRate AudioSampleRate { get; set; } = AudioSampleRate.Default;

        /// <summary>
        ///     The maximum duration
        /// </summary>
        public TimeSpan? MaxVideoDuration { get; set; }

        /// <summary>
        ///     The frame to begin seeking from.
        /// </summary>
        public TimeSpan? Seek { get; set; }

        /// <summary>
        ///     Predefined audio and video options for various file formats,
        ///     <para>Can be used in conjunction with <see cref="TargetStandard" /> option</para>
        /// </summary>
        public Target Target { get; set; } = Target.Default;

        /// <summary>
        ///     Predefined standards to be used with the <see cref="Target" /> option
        /// </summary>
        public TargetStandard TargetStandard { get; set; } = TargetStandard.Default;

        /// <summary>
        ///     Video aspect ratios
        /// </summary>
        public VideoAspectRatio VideoAspectRatio { get; set; } = VideoAspectRatio.Default;

        /// <summary>
        ///     Video bit rate in kbit/s
        /// </summary>
        public int? VideoBitRate { get; set; } = null;
        public int? VideoMinBitRate { get; set; } = null;
        public int? VideoMaxBitRate { get; set; } = null;
        public int? VideoBufferBitRate { get; set; } = null;

        /// <summary>
        ///     Video frame rate
        /// </summary>
        //public int? VideoFps { get; set; } = null;
        public string VideoFps { get; set; } = null;

        /// <summary>
        ///     Video sizes
        /// </summary>
        public VideoSize VideoSize { get; set; } = VideoSize.Default;

        /// <summary>
        ///     Custom Width when VideoSize.Custom is set
        /// </summary>
        public int? CustomWidth { get; set; }

        /// <summary>
        ///     Custom Height when VideoSize.Custom is set
        /// </summary>
        public int? CustomHeight { get; set; }

        /// <summary>
        ///     Specifies an optional rectangle from the source video to crop
        /// </summary>
        public CropRectangle SourceCrop { get; set; }

        /// <summary>
        ///     Specifies wheter or not to use H.264 Baseline Profile
        /// </summary>
        public bool BaselineProfile { get; set; }

        /// <summary>
        ///     <para> --- </para>
        ///     <para> Cut audio / video from existing media                </para>
        ///     <para> Example: To cut a 15 minute section                  </para>
        ///     <para> out of a 30 minute video starting                    </para>
        ///     <para> from the 5th minute:                                 </para>
        ///     <para> The start position would be: TimeSpan.FromMinutes(5) </para>
        ///     <para> The length would be:         TimeSpan.FromMinutes(15)</para>
        /// </summary>
        /// <param name="seekToPosition">
        ///     <para>Specify the position to seek to,                  </para>
        ///     <para>if you wish to begin the cut starting             </para>
        ///     <para>from the 5th minute, use: TimeSpan.FromMinutes(5);</para>
        /// </param>
        /// <param name="length">
        ///     <para>Specify the length of the video to cut,           </para>
        ///     <para>to cut out a 15 minute duration                   </para>
        ///     <para>simply use: TimeSpan.FromMinutes(15);             </para>
        /// </param>
        public void CutMedia(TimeSpan seekToPosition, TimeSpan length)
        {
            Seek = seekToPosition;
            MaxVideoDuration = length;
        }

        public DerOptimist.RenderSettingsItem renderSettingsItem { get; set; }
    }
}