using System;
using FFmpeg.NET.Enums;
using System.Collections.ObjectModel;
using Newtonsoft.Json;
using System.Diagnostics;
using System.IO;
using System.Windows.Input;
using System.Windows;

namespace DerOptimist
{
    public class BaseSettings
    {
        public Guid guid { get; set; }
        public string Name { get; set; }
        public VideoCodecEntry EncoderVideo { get; set; }
        public AudioCodecEntry EncoderAudio { get; set; }
        public int QualityVideo { get; set; }
        public int QualityAudio { get; set; }
        public bool ReplaceAudio { get; set; } = false;
        public bool ResizeVideo { get; set; } = false;
        public string ReplaceAudioFileName { get; set; }
        public int Width { get; set; }
        public int Height { get; set; }
        public string PixFmt { get; set; }
        public FrameRates.FrameRateEntry FrameRate { get; set; } = null;
        public bool ApplyLUT { get; set; } = false;
        public string LUT { get; set; } = "";
        public bool BurnTimeCode { get; set; } = false;
        public bool SetTimeCode { get; set; } = false;
        public string TCh { get; set; } = "00";
        public string TCm { get; set; } = "00";
        public string TCs { get; set; } = "00";
        public string TCf { get; set; } = "00";
        public MediaType MediaType { get; set; } = MediaType.Unknown;
        public AudioSampleRate AudioSampleRate { get; set; } = AudioSampleRate.Default;
        //public string FfmpegParameters { get; set; }

        public BaseSettings()
        {
            NewGuid();
            Name = guid.ToString();
        }

        /// <summary>
        /// Performs a DeepClone on the current object and replaces the cloned guid with a fresh one
        /// </summary>
        /// <returns>A clone with a new guid</returns>
        public virtual BaseSettings GetBaseClone()
        {
            BaseSettings bs = this.DeepClone();
            bs.guid = Guid.NewGuid();
            if (bs.Name == null || bs.Name == "") Name = bs.ToString();
            return bs;
        }

        public virtual void NewGuid()
        {
            this.guid = Guid.NewGuid();
        }

        public virtual string ToJson()
        {
            var rh = this.GetBaseClone();
            var json = JsonConvert.SerializeObject(rh);
            return json;
        }

        public static BaseSettings FromJson(string json)
        {
            BaseSettings rh;
            try
            {
                rh = JsonConvert.DeserializeObject<BaseSettings>(json);
            }
            catch (Exception ex)
            {
                Debug.WriteLine("Exception converting RenderHistoryEntry from JSON");
                Debug.WriteLine($"JSON: {json}");
                Debug.WriteLine($"Exception: {ex.Message}");
                return null;
            }
            return rh;
        }

        public static BaseSettings FromJsonFile(string path)
        {
            try
            {
                var s = File.ReadAllText(path);
                if (s != null)
                {
                    return FromJson(s);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
            return null;
        }

        public static bool ToJsonFileWithDialog(BaseSettings renderSettings)
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
                    return ToJsonFile(renderSettings, filename);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
                return false;
            }
            return true;
        }

        public static bool ToJsonFile(BaseSettings renderSettings, string path)
        {
            try
            {
                var jsonPreset = renderSettings.ToJson();
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
