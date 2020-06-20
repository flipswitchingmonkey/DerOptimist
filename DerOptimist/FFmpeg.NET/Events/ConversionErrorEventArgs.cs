using System;
using System.Collections.Generic;
using FFmpeg.NET.Exceptions;

namespace FFmpeg.NET.Events
{
    public class ConversionErrorEventArgs : EventArgs
    {
        public ConversionErrorEventArgs(FFmpegException exception, MediaFile input, MediaFile output, List<string> messages, ConversionOptions options = null)
        {
            Exception = exception;
            Input = input;
            Output = output;
            Messages = messages;
            Options = options;
        }

        public ConversionErrorEventArgs(FFmpegException exception, MediaFile input, MediaFile output, ConversionOptions options = null)
        {
            Exception = exception;
            Input = input;
            Output = output;
            Messages = new List<string>();
            Options = options;
        }

        public FFmpegException Exception { get; }
        public MediaFile Input { get; }
        public MediaFile Output { get; }
        public List<string> Messages { get; }
        public ConversionOptions Options { get; }
    }
}