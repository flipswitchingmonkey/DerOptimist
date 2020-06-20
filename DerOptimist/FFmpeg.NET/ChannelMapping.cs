using System;
using System.Collections.Generic;
using System.Text;

namespace FFmpeg.NET
{
    public class ChannelMapping
    {
        int Input { get; set; }
        int Output { get; set; }

        public ChannelMapping()
        {
            Input = 0;
            Output = 0;
        }

        public ChannelMapping(int input, int output)
        {
            Input = input;
            Output = output;
        }

        public override string ToString()
        {
            return ($"map {Input}:{Output}");
        }
    }
}
