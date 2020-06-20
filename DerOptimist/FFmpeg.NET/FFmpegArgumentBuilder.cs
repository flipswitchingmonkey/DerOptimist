using FFmpeg.NET.Enums;
using System;
using System.Globalization;
using System.Text;

namespace FFmpeg.NET
{
    internal class FFmpegArgumentBuilder
    {
        public string Build(FFmpegParameters parameters)
        {
            if (parameters.HasCustomArguments)
                return parameters.CustomArguments;

            switch (parameters.Task)
            {
                case FFmpegTask.Convert:
                    return Convert(parameters.InputFile, parameters.OutputFile, parameters.ConversionOptions);

                case FFmpegTask.GetMetaData:
                    return GetMetadata(parameters.InputFile);

                case FFmpegTask.GetThumbnail:
                    return GetThumbnail(parameters.InputFile, parameters.OutputFile, parameters.ConversionOptions);
                default:
                    throw new ArgumentOutOfRangeException();
            }
        }

        private static string GetMetadata(MediaFile inputFile) => $"-i \"{inputFile.FileInfo.FullName}\" -f ffmetadata -";

        private static string GetThumbnail(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var defaultTimeSpan = TimeSpan.FromSeconds(1);
            var commandBuilder = new StringBuilder();

            commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", conversionOptions?.Seek.GetValueOrDefault(defaultTimeSpan).TotalSeconds ?? defaultTimeSpan.TotalSeconds);

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.FileInfo.FullName);
            commandBuilder.AppendFormat(" -vframes {0} ", 1);

            return commandBuilder.AppendFormat(" \"{0}\" ", outputFile.FileInfo.FullName).ToString();
        }

        private static string Convert(MediaFile inputFile, MediaFile outputFile, ConversionOptions conversionOptions)
        {
            var commandBuilder = new StringBuilder();

            // Default conversion
            if (conversionOptions == null)
                return commandBuilder.AppendFormat(" -i \"{0}\" \"{1}\" ", inputFile.FileInfo.FullName, outputFile.FileInfo.FullName).ToString();

            if (conversionOptions.PreArgs != null)
                commandBuilder.AppendFormat(" {0} ", conversionOptions.PreArgs);

            //if (conversionOptions.PreArgs.Count > 0)
            //{
            //    string preargs = "";
            //    for (int i = 0; i < conversionOptions.PreArgs.Count; i++)
            //    {
            //        preargs += $" {conversionOptions.PreArgs[i]} ";
            //    }
            //    commandBuilder.AppendFormat(preargs);
            //}

            // Input frame rate
            if (conversionOptions.InputFps != null)
                commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -framerate {0} ", conversionOptions.InputFps.ToString());

            // Media seek position
            if (conversionOptions.Seek != null)
                commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", conversionOptions.Seek.Value.TotalSeconds);

            commandBuilder.AppendFormat(" -i \"{0}\" ", inputFile.FileInfo.FullName);

            if (conversionOptions.SecondInput != null)
            {
                if (conversionOptions.Seek != null)
                    commandBuilder.AppendFormat(CultureInfo.InvariantCulture, " -ss {0} ", conversionOptions.Seek.Value.TotalSeconds);

                commandBuilder.AppendFormat(" -i \"{0}\" ", conversionOptions.SecondInput.FileInfo.FullName);
            }

            foreach (var mapping in conversionOptions.Mappings)
            {
                commandBuilder.AppendFormat(" -{0} ", mapping.ToString());
            }
            // Physical media conversion (DVD etc)
            if (conversionOptions.Target != Target.Default)
            {
                commandBuilder.Append(" -target ");
                if (conversionOptions.TargetStandard != TargetStandard.Default)
                {
                    commandBuilder.AppendFormat(" {0}-{1} \"{2}\" ", conversionOptions.TargetStandard.ToString().ToLowerInvariant(), conversionOptions.Target.ToString().ToLowerInvariant(), outputFile.FileInfo.FullName);

                    return commandBuilder.ToString();
                }

                commandBuilder.AppendFormat(" {0} \"{1}\" ", conversionOptions.Target.ToString().ToLowerInvariant(), outputFile.FileInfo.FullName);

                return commandBuilder.ToString();
            }

            if (conversionOptions.TimeCode != null)
            {
                commandBuilder.AppendFormat(" -timecode {0} ", conversionOptions.TimeCode);
            }

            // Video Encoder
            if (conversionOptions.VideoEncoder != null)
            {
                if (conversionOptions.VideoEncoder.Encoder == VideoCodec.None)
                {
                    commandBuilder.AppendFormat(" -vn");
                }
                else
                {
                    var encoderDefaults = conversionOptions.VideoEncoder;
                    commandBuilder.AppendFormat(" -c:v {0} {1}", encoderDefaults.Encoder, encoderDefaults.OutputArgs);
                    if (encoderDefaults.QualityMode != null)
                    {
                        if (conversionOptions.QualityVideo >= encoderDefaults.QualityMin && conversionOptions.QualityVideo <= encoderDefaults.QualityMax)
                        {
                            commandBuilder.AppendFormat(" -{0} {1}{2}", encoderDefaults.QualityMode, conversionOptions.QualityVideo.ToString(), encoderDefaults.QualityPostfix);
                        }
                        else
                        {
                            commandBuilder.AppendFormat(" -{0} {1}{2}", encoderDefaults.QualityMode, encoderDefaults.QualityDefault.ToString(), encoderDefaults.QualityPostfix);
                        }
                        if (conversionOptions.VideoMinBitRate != null)
                        {
                            commandBuilder.AppendFormat(" -minrate {0}{1}", conversionOptions.VideoMinBitRate.ToString(), encoderDefaults.QualityPostfix);
                        }
                        if (conversionOptions.VideoMaxBitRate != null)
                        {
                            commandBuilder.AppendFormat(" -maxrate {0}{1}", conversionOptions.VideoMaxBitRate.ToString(), encoderDefaults.QualityPostfix);
                        }
                        if (conversionOptions.VideoBufferBitRate != null)
                        {
                            commandBuilder.AppendFormat(" -bufsize {0}{1}", conversionOptions.VideoBufferBitRate.ToString(), encoderDefaults.QualityPostfix);
                        }
                    }
                    if (conversionOptions.PixFmt != "")
                    {
                        commandBuilder.AppendFormat(" -pix_fmt {0}", conversionOptions.PixFmt);
                    }
                    else if (encoderDefaults.PixFmt != "")
                    {
                        commandBuilder.AppendFormat(" -pix_fmt {0}", encoderDefaults.PixFmt);
                    }
                }
            }

            // Audio Encoder
            if (conversionOptions.AudioEncoder != null)
            {
                if (conversionOptions.AudioEncoder.Encoder == AudioCodec.None)
                {
                    commandBuilder.AppendFormat(" -an");
                }
                else
                {
                    var encoderDefaults = conversionOptions.AudioEncoder;
                    commandBuilder.AppendFormat(" -c:a {0} {1}", encoderDefaults.Encoder, encoderDefaults.OutputArgs);
                    if (encoderDefaults.QualityMode != null)
                    {
                        if (conversionOptions.QualityAudio >= encoderDefaults.QualityMin && conversionOptions.QualityAudio <= encoderDefaults.QualityMax)
                        {
                            commandBuilder.AppendFormat(" -{0} {1}{2}", encoderDefaults.QualityMode, conversionOptions.QualityAudio.ToString(), encoderDefaults.QualityPostfix);
                        }
                        else
                        {
                            commandBuilder.AppendFormat(" -{0} {1}{2}", encoderDefaults.QualityMode, encoderDefaults.QualityDefault.ToString(), encoderDefaults.QualityPostfix);
                        }
                    }
                }
            }

            // Audio bit rate
            if (conversionOptions.AudioBitRate != null)
                commandBuilder.AppendFormat(" -ab {0}k", conversionOptions.AudioBitRate);

            // Audio sample rate
            if (conversionOptions.AudioSampleRate != AudioSampleRate.Default)
                commandBuilder.AppendFormat(" -ar {0} ", conversionOptions.AudioSampleRate.ToString().Replace("Hz", ""));

            // Maximum video duration
            if (conversionOptions.MaxVideoDuration != null)
                commandBuilder.AppendFormat(" -t {0} ", conversionOptions.MaxVideoDuration);

            // Video bit rate
            if (conversionOptions.VideoBitRate != null)
                commandBuilder.AppendFormat(" -b {0}k ", conversionOptions.VideoBitRate);

            // Video frame rate
            if (conversionOptions.VideoFps != null)
                commandBuilder.AppendFormat(" -r {0} ", conversionOptions.VideoFps);

            // Video size / resolution
            if (conversionOptions.VideoSize == VideoSize.Custom)
            {
                commandBuilder.AppendFormat(" -vf \"scale={0}:{1}\" ", conversionOptions.CustomWidth ?? -2, conversionOptions.CustomHeight ?? -2);
            }
            else if (conversionOptions.VideoSize != VideoSize.Default)
            {
                var size = conversionOptions.VideoSize.ToString().ToLowerInvariant();
                if (size.StartsWith("_"))
                    size = size.Replace("_", "");
                if (size.Contains("_"))
                    size = size.Replace("_", "-");

                commandBuilder.AppendFormat(" -s {0} ", size);
            }

            // Video Filters
            if (conversionOptions.VideoFilters.Count > 0)
            {
                string vf = " -vf \"";
                for (int i = 0; i < conversionOptions.VideoFilters.Count; i++)
                {
                    vf += conversionOptions.VideoFilters[i];
                    if (i < conversionOptions.VideoFilters.Count - 1)
                    {
                        vf += ",";
                    }
                }
                vf += "\" ";
                commandBuilder.AppendFormat(vf);
            }


            // Video aspect ratio
            if (conversionOptions.VideoAspectRatio != VideoAspectRatio.Default)
            {
                var ratio = conversionOptions.VideoAspectRatio.ToString();
                ratio = ratio.Substring(1);
                ratio = ratio.Replace("_", ":");

                commandBuilder.AppendFormat(" -aspect {0} ", ratio);
            }

            // Video cropping
            if (conversionOptions.SourceCrop != null)
            {
                var crop = conversionOptions.SourceCrop;
                commandBuilder.AppendFormat(" -filter:v \"crop={0}:{1}:{2}:{3}\" ", crop.Width, crop.Height, crop.X, crop.Y);
            }

            if (conversionOptions.BaselineProfile)
                commandBuilder.Append(" -profile:v baseline ");

            return commandBuilder.AppendFormat(" \"{0}\" ", outputFile.FileInfo.FullName).ToString();
        }
    }
}