using System;
using FFmpeg.NET.Enums;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows;
using System.Threading;
using FFmpeg.NET;

namespace DerOptimist
{
    public class RenderSettingsCollection : KeyedCollection<Guid, RenderSettingsItem>
    {
        protected override Guid GetKeyForItem(RenderSettingsItem item)
        {
            // In this example, the key is the part number.
            return item.guid;
        }
    }

    public enum RenderQueueItemStatus
    {
        None,
        Queued,
        Encoding,
        Stopped,
        Finished,
        Error,
        Waiting,
        Preview
    }

    [Serializable]
    public class RenderSettingsItem
    {
        public Guid guid { get; set; }
        public RenderQueueItemStatus Status {get; set;} = RenderQueueItemStatus.None;
        public double Progress { get; set; } = 0;

        [JsonIgnore] [NonSerialized] public CancellationTokenSource cancellationTokenSource;
        [JsonIgnore] [NonSerialized] public MediaFile Task;
        [JsonIgnore] [NonSerialized] public MediaFile InputFile;
        [JsonIgnore] [NonSerialized] public MediaFile OutputFile;
        [JsonIgnore] [NonSerialized] public ConversionOptions Options;
        [JsonIgnore] [NonSerialized] public string Msg;
        [JsonIgnore] [NonSerialized] public bool Queued = false;

        public string Name { get; set; }
        public VideoCodecEntry EncoderVideo { get; set; }
        public AudioCodecEntry EncoderAudio { get; set; }
        public int QualityVideo {get; set;}
        public int QualityAudio { get; set; }
        [JsonIgnore] public string SourcePath { get; set; }
        [JsonIgnore] public string OutputPath { get; set; }
        [JsonIgnore] public bool IsPreview { get; set; }
        [JsonIgnore] public bool IsSelected { get; set; } = false;
        [JsonIgnore] public bool UseRange { get; set; } = false;
        public bool ReplaceAudio { get; set; } = false;
        public bool ResizeVideo { get; set; } = false;
        public string ReplaceAudioFileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string PixFmt { get; set; }
        [JsonIgnore] public TimeSpan RangeStart { get; set; }
        [JsonIgnore] public TimeSpan RangeEnd { get; set; }
        [JsonIgnore] public TimeSpan Duration { get; set; }
        public FrameRates.FrameRateEntry FrameRate { get; set; } = null;
        public bool ApplyLUT { get; set; } = false;
        public string LUT { get; set; } = "";
        public bool ApplyGamma { get; set; } = false;
        public string Gamma { get; set; } = "1.0";
        public string Color_Range_In { get; set; } = "default";
        public string Color_Range_Out { get; set; } = "default";
        public string ExtraVideoFilters { get; set; } = "";
        public bool BurnTimeCode { get; set; } = false;
        public bool SetTimeCode { get; set; } = false;
        public string TCh { get; set; } = "00";
        public string TCm { get; set; } = "00";
        public string TCs { get; set; } = "00";
        public string TCf { get; set; } = "00";
        public MediaType MediaType { get; set; } = MediaType.Unknown;
        public AudioSampleRate AudioSampleRate { get; set; } = AudioSampleRate.Default;
        [JsonIgnore] public string Log { get; set; } = "";

        [JsonIgnore] public string FfmpegParameters { get; set; }

        public RenderSettingsItem()
        {
            NewGuid();
            Name = guid.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="PreviewPath">Path to the preview video file</param>
        public RenderSettingsItem(string PreviewPath)
        {
            NewGuid();
            this.OutputPath = PreviewPath;
            Name = guid.ToString();
        }

        /// <summary>
        /// Performs a DeepClone on the current object and replaces the cloned guid with a fresh one
        /// </summary>
        /// <returns>A clone with a new guid</returns>
        public RenderSettingsItem GetClone()
        {
            RenderSettingsItem rh = this.DeepClone();
            rh.guid = Guid.NewGuid();
            if (rh.Name == null || rh.Name == "") Name = rh.ToString();
            return rh;
        }

        public RenderSettingsItem NewGuid()
        {
            this.guid = Guid.NewGuid();
            return this;
        }

        public string ToJson(bool sanitize=true)
        {
            var rh = this.GetClone();
            if (sanitize)
            {
                rh.SourcePath = null;
                rh.OutputPath = null;
                rh.IsPreview = false;
                rh.IsSelected = false;
            }
            var json = JsonConvert.SerializeObject(rh, Formatting.Indented);
            Debug.WriteLine(json);
            return json;
        }

        //public RenderQueueItem ToRenderQueueItem()
        //{
        //    RenderQueueItem rqi = (RenderQueueItem)this;
        //    rqi.Status = RenderQueueItemStatus.Queued;
        //    return rqi;
        //}

        public static RenderSettingsItem FromJson(string json, bool sanitize=true)
        {
            RenderSettingsItem rh;
            try
            {
                rh = JsonConvert.DeserializeObject<RenderSettingsItem>(json);
            }
            catch(Exception ex)
            {
                Debug.WriteLine("Exception converting RenderHistoryEntry from JSON");
                Debug.WriteLine($"JSON: {json}");
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
            if (sanitize)
            {
                rh.guid = Guid.NewGuid();
                rh.SourcePath = null;
                rh.OutputPath = null;
                rh.IsPreview = false;
                rh.IsSelected = false;
            }
            return rh;
        }

        public static RenderSettingsItem FromJsonFile(string path, bool sanitize = true)
        {
            try
            {
                var s = File.ReadAllText(path);
                if (s != null)
                {
                    return FromJson(s, sanitize);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static bool ToJsonFileWithDialog(RenderSettingsItem renderSettings, bool sanitize = true)
        {
            try
            {
                Microsoft.Win32.SaveFileDialog saveFileDialog = new Microsoft.Win32.SaveFileDialog();
                saveFileDialog.InitialDirectory = Properties.Settings.Default.PresetFolderPath;
                saveFileDialog.Filter = "Presets(*.json)|*.json"; ;
                if (saveFileDialog.ShowDialog() == true)
                {
                    var filename = saveFileDialog.FileName;
                    if (Path.GetExtension(filename) != ".json") filename = $"{Path.GetFileNameWithoutExtension(filename)}.json";
                    if (File.Exists(filename))
                    {
                        FileInfo fi = new FileInfo(filename);
                        var res = MessageBox.Show($"File {fi.Name} exists, would you like to overwrite it?", "File exists", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                        if (res == MessageBoxResult.No)
                        {
                            return false;
                        }
                    }

                    renderSettings.Name = Path.GetFileNameWithoutExtension(filename);
                    return ToJsonFile(renderSettings, filename, sanitize);
                    //var jsonPreset = renderSettings.ToJson(sanitize:sanitize);
                    //System.IO.File.WriteAllText(filename, jsonPreset);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static bool ToJsonFile(RenderSettingsItem renderSettings, string path, bool sanitize = true)
        {
            try
            {
                var jsonPreset = renderSettings.ToJson(sanitize: sanitize);
                System.IO.File.WriteAllText(path, jsonPreset);
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

    }
}
