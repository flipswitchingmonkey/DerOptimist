using System;
using System.Collections.ObjectModel;
using Newtonsoft.Json;

namespace FFmpeg.NET.Enums
{
    public static class FrameRates
    {
        public static FrameRateCollectionType FrameRateCollection;

        static FrameRates()
        {
            FrameRateCollection = new FrameRateCollectionType();
            FrameRateCollection.Add(new FrameRateEntry("23.976", "24000/1001"));
            FrameRateCollection.Add(new FrameRateEntry("25", "25"));
            FrameRateCollection.Add(new FrameRateEntry("29.97", "30000/1001"));
            FrameRateCollection.Add(new FrameRateEntry("30", "30"));
            FrameRateCollection.Add(new FrameRateEntry("50", "50"));
            FrameRateCollection.Add(new FrameRateEntry("60", "60"));
            FrameRateCollection.Add(new FrameRateEntry("100", "100"));
            FrameRateCollection.Add(new FrameRateEntry("120", "120"));
        }

        public class FrameRateCollectionType : KeyedCollection<string, FrameRateEntry>
        {
            protected override string GetKeyForItem(FrameRateEntry item)
            {
                // In this example, the key is the part number.
                return item.Key;
            }
        }

        [Serializable]
        public class FrameRateEntry
        {
            public string Key { get; set; }
            public string DisplayName { get; set; }
            // ffmpeg compatible way to express frame rate (e.g. as fraction)
            public string CommandName { get; set; }

            public FrameRateEntry(string name)
            {
                DisplayName = name;
                Key = name;
                CommandName = name;
            }

            public FrameRateEntry(string name, string cmd)
            {
                DisplayName = name;
                Key = name;
                CommandName = cmd;
            }

            [JsonConstructor]
            public FrameRateEntry(string key, string name, string cmd)
            {
                DisplayName = name;
                CommandName = cmd;
                Key = key;
            }
            public override string ToString()
            {
                return DisplayName;
            }
        }
    }
}
