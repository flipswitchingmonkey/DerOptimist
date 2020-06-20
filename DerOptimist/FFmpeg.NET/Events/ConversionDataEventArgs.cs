using System;

namespace FFmpeg.NET.Events
{
    public class ConversionDataEventArgs : EventArgs
    {
        public ConversionDataEventArgs(string data, MediaFile input, MediaFile output, ConversionOptions options = null)
        {
            Data = data;
            Input = input;
            Output = output;
        }

        public string Data { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public ConversionOptions Options { get; }
    }
}