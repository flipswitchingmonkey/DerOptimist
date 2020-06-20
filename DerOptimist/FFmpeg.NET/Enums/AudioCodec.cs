using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace FFmpeg.NET.Enums
{
    public static class AudioCodec
    {
        public static AudioCodecCollection SettingsCollection;

        public static KeyValuePair<string, AudioSampleRate>[] sampleRates = {
            new KeyValuePair<string, AudioSampleRate>("Default", AudioSampleRate.Default),
            new KeyValuePair<string, AudioSampleRate>("22KHz", AudioSampleRate.Hz22050),
            new KeyValuePair<string, AudioSampleRate>("44KHz", AudioSampleRate.Hz44100),
            new KeyValuePair<string, AudioSampleRate>("48KHz", AudioSampleRate.Hz48000)
        };
        public static KeyValuePair<string, AudioSampleRate>[] SampleRates
        {
            get
            {
                return sampleRates;
            }
        }

        public static string AAC_VBR { get { return "AAC-VBR"; } }
        public static string AAC_CBR { get { return "AAC-CBR"; } }
        public static string Vorbis { get { return "Vorbis"; } }
        public static string FLAC { get { return "FLAC"; } }
        public static string WavPack { get { return "WavPack"; } }
        public static string Opus { get { return "Opus"; } }
        public static string MP2 { get { return "MP2"; } }
        public static string MP3 { get { return "MP3"; } }
        public static string Copy { get { return "Copy"; } }
        public static string None { get { return "None"; } }
        public static string Default = AAC_VBR;

        static AudioCodec() {
            SettingsCollection = new AudioCodecCollection();
            SettingsCollection.Add(new AudioCodecEntry("Copy") { Encoder = "copy -map 0", FileExtension = null, QualityMode=null });
            SettingsCollection.Add(new AudioCodecEntry("None") { Encoder = "None", FileExtension = null, QualityMode = null });
            SettingsCollection.Add(new AudioCodecEntry("AAC-CBR") {Encoder = "aac", FileExtension = ".m4a", QualityMode = "b:a", QualityMin = 32, QualityMax = 320, QualityDefault = 128, QualityPostfix = "K", QualityStep = 8 });
            SettingsCollection.Add(new AudioCodecEntry("AAC-VBR") { Encoder = "aac", FileExtension = ".m4a", QualityMode = "q:a", QualityMin = 32, QualityMax = 320, QualityDefault = 128, QualityStep = 8 });
            SettingsCollection.Add(new AudioCodecEntry("MP3") { Encoder = "libmp3lame", FileExtension = ".mp3", QualityMode = "b:a", QualityMin = 32, QualityMax = 320, QualityDefault = 160, QualityPostfix="K", QualityStep = 8 });
            SettingsCollection.Add(new AudioCodecEntry("FLAC") { Encoder = "flac", FileExtension = ".flac", OutputArgs="-strict experimental", QualityMode= "compression_level", QualityMin = 0, QualityMax = 12, QualityDefault = 5 });
            SettingsCollection.Add(new AudioCodecEntry("Vorbis") { Encoder = "libvorbis", FileExtension = ".ogg", QualityMode = "q:a", QualityMin = 0, QualityMax = 10, QualityDefault = 3 });
            SettingsCollection.Add(new AudioCodecEntry("WavPack") { Encoder = "wavpack", FileExtension = ".wv", QualityMode = "compression_level", QualityMin = 0, QualityMax = 8, QualityDefault = 0 });
            SettingsCollection.Add(new AudioCodecEntry("Opus") { Encoder = "libopus", FileExtension=".opus", OutputArgs = "-strict experimental", QualityMode = "b:a", QualityMin = 32000, QualityMax = 320000, QualityDefault = 128000, QualityStep = 8000 });
            SettingsCollection.Add(new AudioCodecEntry("MP2") { Encoder = "libtwolame", FileExtension = ".m2a", QualityMode = "b:a", QualityMin = 32, QualityMax = 320, QualityDefault = 128, QualityStep = 8 });
        }

        public static AudioCodecEntry Settings(string codecName)
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

    
    public class AudioCodecCollection : KeyedCollection<string, AudioCodecEntry>
    {
        protected override string GetKeyForItem(AudioCodecEntry item)
        {
            // In this example, the key is the part number.
            return item.Name;
        }

    }

    [Serializable]
    public class AudioCodecEntry
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

        public string QualityMode {get; set;} = null;
        public int QualityMin {get;set;} = 0;
        public int QualityMax {get;set;} = 51;
        public int QualityDefault {get;set;} = 18;
        public int QualityStep { get; set; } = 1;
        public string QualityPostfix { get; set; } = "";
        /// <summary>
        /// Summary of various codec specific settings to be used with ffmpeg.
        /// </summary>
        /// <param name="Name">A unique name given to this entry. This is used as the collection Key value.</param>
        [JsonConstructor]
        public AudioCodecEntry(string Name)
        {
            this.Name = Name;
        }
        public override string ToString()
        {
            return Name;
        }
    }
}
