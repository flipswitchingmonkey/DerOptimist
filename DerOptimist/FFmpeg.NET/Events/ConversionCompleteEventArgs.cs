using System;
using System.Collections.Generic;

namespace FFmpeg.NET.Events
{
    public class ConversionCompleteEventArgs : EventArgs
    {
        public ConversionCompleteEventArgs(MediaFile input, MediaFile output, ConversionOptions options = null)
        {
            Input = input;
            Output = output;
            Messages = new List<string>();
            Options = options;
        }

        public ConversionCompleteEventArgs(MediaFile input, MediaFile output, List<string> messages, ConversionOptions options=null)
        {
            Input = input;
            Output = output;
            Messages = messages;
            Options = options;
        }

        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public List<string> Messages { get; }
        public ConversionOptions Options { get; }
    }
}