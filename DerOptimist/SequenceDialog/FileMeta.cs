using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using MediaInfoDotNet;

namespace DerOptimist
{
    [Serializable]
    public class FileMeta
    {
        public Guid Id;
        public FileSystemInfo Info { get; set; }
        [NonSerialized]
        public MediaInfoDotNet.MediaFile mi;
        public MediaType MediaType;
        public int First { get; set; }
        public int Last { get; set; }
        public string BaseName { get; set; }
        public string BaseFileName { get; set; }
        public string AfterCounter { get; set; }
        public string CounterString { get; set; }
        public string PatternString { get; set; }
        public string FAIcon { get; set; }
        public bool IsDirectory
        {
            get
            {
                return MediaType == MediaType.Directory;
            }
        }
        public int Digits { get; set; }
        List<string> FileList { get; set; }

        public string DisplayName
        {
            get
            {
                if (MediaType == MediaType.Up)
                {
                    return "..";
                }
                else if (MediaType == MediaType.ImageSequence)
                {
                    string hashes = "";
                    for (int i = 0; i < Digits; i++)
                    {
                        hashes += "#";
                    }

                    //return $"{PatternString} ({First}..{Last})";
                    return $"{BaseFileName}{hashes}{Info.Extension} ({First}..{Last})";
                }
                else if (MediaType == MediaType.Directory)
                {
                    return $"[{Info.Name}]";
                }
                else
                {
                    return Info.Name;
                }
            }
        }

        public string DisplaySize
        {
            get
            {
                if (MediaType == MediaType.ImageSequence)
                {
                    long size = 0;
                    for (int i = First; i <= Last; i++)
                    {
                        string path = GetPathWithCounter(i);
                        if (File.Exists(path))
                        {
                            var fi = new FileInfo(path);
                            size += fi.Length;
                        }
                    }
                    //return (size / 1024.0 / 1024.0) < 10.0 ? $"{(size / 1024.0).ToString("F2")} KB" : $"{(size / 1024.0 / 1024.0).ToString("F2")} MB";
                    return $"{(size / 1024.0).ToString("F2")} KB";
                }
                else if (MediaType == MediaType.Directory)
                {
                    return $"Directory";
                }
                else if (MediaType == MediaType.Up)
                {
                    return $"Up";
                }
                else
                {
                    long size = (Info as FileInfo).Length;
                    //return (size / 1024.0 / 1024.0) < 10.0 ? $"{(size / 1024.0).ToString("F2")} KB" : $"{(size / 1024.0 / 1024.0).ToString("F2")} MB";
                    return $"{(size / 1024.0).ToString("F2")} KB";
                }
            }
        }

        public long Size
        {
            get
            {
                if (MediaType == MediaType.ImageSequence)
                {
                    long size = 0;
                    for (int i = First; i <= Last; i++)
                    {
                        string path = GetPathWithCounter(i);
                        if (File.Exists(path))
                        {
                            var fi = new FileInfo(path);
                            size += fi.Length;
                        }
                    }
                    return size;
                }
                else if (MediaType == MediaType.Directory)
                {
                    return 0;
                }
                else
                {
                    return (Info as FileInfo).Length;
                }
            }
        }

        public string ExtensionLower {
            get {
                return Info.Extension?.ToLower() ?? "";
            }
        }

        public string ExtensionLowerNoDot
        {
            get
            {
                return ExtensionLower.Length > 0 ? ExtensionLower.Remove(0, 1) : "";
            }
        }

        public string ExtensionNoDot
        {
            get
            {
                return this.Info.Extension?.Remove(0, 1) ?? "";
            }
        }

        public FileMeta()
        {
            this.Id = Guid.NewGuid();
        }

        public FileMeta(string fileName, MediaType newMediaType = MediaType.Unknown, string Icon = "")
        {
            this.Id = Guid.NewGuid();
            this.MediaType = newMediaType;
            this.Info = new FileInfo(fileName);
            this.FAIcon = Icon;
        }

        public FileMeta(FileInfo fileInfo, MediaType newMediaType = MediaType.Unknown, string Icon = "")
        {
            this.Id = Guid.NewGuid();
            this.MediaType = newMediaType;
            this.Info = fileInfo;
            this.FAIcon = Icon;
        }

        public string GetPathWithCounter(int counter)
        {
            return $"{this.BaseName}{counter.ToString("D"+this.Digits.ToString())}{this.AfterCounter}{this.ExtensionLower}";
        }

        /// <summary>
        /// Returns an ffmpeg compatible string representing the image squence
        /// </summary>
        /// <returns></returns>
        public string GetSequenceString()
        {
            string counter = $"%0{this.Digits}d";
            string dir = Path.GetDirectoryName(this.Info.FullName);
            return Path.Combine(dir, this.BaseName + counter + this.AfterCounter + this.Info.Extension);
        }

        /// <summary>
        /// Returns an ffmpeg compatible string representing the image squence
        /// </summary>
        /// <returns></returns>
        public string GetFfmpegSourceString()
        {
            if (MediaType == MediaType.ImageSequence)
            {
                string counter = $"%0{this.Digits}d";
                string dir = Path.GetDirectoryName(this.Info.FullName);
                return Path.Combine(dir, this.BaseName + counter + this.AfterCounter + this.Info.Extension);
            }
            else
            {
                return Info.FullName;
            }
        }

        public Uri GetUri()
        {
            return new Uri(GetFfmpegSourceString());
        }

        public Uri GetSequenceUri()
        {
            return new Uri(GetFfmpegSourceString());
        }

        public FileMeta GetCopy()
        {
            FileMeta copy = this.DeepClone();
            copy.Id = Guid.NewGuid();
            return copy;
        }

        public static FileMeta Create(string fileName, bool groupSequences = true)
        {
            if (fileName == null)
                return null;
            var ignore = new List<string>();
            return Create(fileName, ref ignore, groupSequences);
        }

        /// <summary>
        /// Factory function to create a new FileMeta by analyzing the file type and whether it is part of a sequence of files. This is the recommended way to create a new FileMeta object.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="ignoreList"></param>
        /// <returns></returns>
        public static FileMeta Create(string fileName, ref List<string> ignoreList, bool groupSequences=true)
        {
            if (fileName == null)
                return null;
            try
            {
                // check if file or directory, if directory, return a simplified FileMeta
                FileAttributes attr = File.GetAttributes(fileName);

                if (attr.HasFlag(FileAttributes.Directory))
                {
                    return new FileMeta() {
                        MediaType = MediaType.Directory,
                        Info = new DirectoryInfo(fileName),
                        FAIcon = MediaIcons.Folder
                    };
                }
            }
            catch (FileNotFoundException)
            {
                return null;
            }
            catch (DirectoryNotFoundException)
            {
                return null;
            }


            //if (!File.Exists(fileName)) return null;

            if (ignoreList == null) ignoreList = new List<string>();

            FileMeta fm = new FileMeta(fileName);
            
            if (fm.Info.Extension == null)
            {
                return null;
            }


            //fm.mi = new MediaInfoDotNet.MediaFile(fileName);

            //if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsSkip).Contains<string>(fm.ExtensionLowerNoDot))
            //if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsSkip).Contains<string>(fm.ExtensionLowerNoDot))
            //{
            //    return null;
            //}
            if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsImage).Contains<string>(fm.ExtensionLowerNoDot))
            //if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsImages).Contains<string>(fm.ExtensionLowerNoDot))
            //if (fm.mi.Image.Count > 0 || fm.mi.General.InternetMediaType.Contains("image") || fm.mi.General.Format == "EXR")
            {
                var digitRegex = new Regex(@"\d+");

                var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
                
                var matches = digitRegex.Matches(fileNameWithoutExtension);

                if (matches.Count > 0)
                {
                    var lastMatch = matches[matches.Count - 1];
                    fm.BaseFileName = fileNameWithoutExtension.Substring(0, lastMatch.Index);
                    fm.AfterCounter = fileNameWithoutExtension.Substring(lastMatch.Index + lastMatch.Length);
                    fm.BaseName = Path.Combine((fm.Info as FileInfo).DirectoryName, fm.BaseFileName);
                    fm.PatternString = digitRegex.Replace(Regex.Escape(fileNameWithoutExtension), @"\d{" + lastMatch.Length.ToString() + "}", 1, lastMatch.Index) + fm.Info.Extension;
                    fm.Digits = lastMatch.Length;
                    fm.CounterString = lastMatch.Value;
                }
                else
                {
                    fm.BaseName = fileName;
                    fm.BaseFileName = fm.Info.Name;
                    fm.PatternString = "";
                    fm.Digits = 0;
                    fm.CounterString = "";
                    fm.AfterCounter = "";
                }

                // if the ignore list contains an entry of the same base filename with the same amount of digits, skip this file
                // this is so there can be multiple sequences of the same base name but with different counter-lengths
                if (ignoreList.Contains(fm.PatternString))
                {
                    return null;
                }

                // files that have no counter at all at its end are considered single images
                // also return if sequences should not be grouped at all
                if (fm.Digits == 0 || groupSequences == false)
                {
                    fm.MediaType = MediaType.Image;
                    fm.FAIcon = MediaIcons.Image;
                    return fm;
                }

                ignoreList.Add(fm.PatternString);

                // search the file's directory for similarly named files
                var allFiles = Directory.EnumerateFiles((fm.Info as FileInfo).DirectoryName);
                var searchPattern = new Regex(fm.PatternString);
                
                // filter all files that have the same base name AND the same digit count
                var similarFiles = allFiles.Where(item => searchPattern.IsMatch(item));

                List<string> sortedList = similarFiles.OrderBy(q => q).ToList();

                if (sortedList.Count == 1)
                {
                    fm.MediaType = MediaType.Image;
                    fm.FAIcon = MediaIcons.Image;
                    return fm;
                }

                matches = digitRegex.Matches(sortedList.First());
                var matchFirst = matches[matches.Count - 1];
                fm.First = int.Parse(matchFirst.Value);

                matches = digitRegex.Matches(sortedList.Last());
                var matchLast = matches[matches.Count - 1];
                fm.Last = int.Parse(matchLast.Value);

                fm.MediaType = MediaType.ImageSequence;
                fm.FAIcon = MediaIcons.Images;
                return fm;
            }
            else if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsVideo).Contains<string>(fm.ExtensionLowerNoDot))
            //else if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsVideo).Contains<string>(fm.ExtensionLowerNoDot))
            //else if (fm.mi.Video.Count > 0 || fm.mi.General.InternetMediaType.Contains("video"))
            {
                fm.MediaType = MediaType.Movie;
                fm.FAIcon = MediaIcons.Video;
                fm.BaseFileName = Path.GetFileNameWithoutExtension(fileName);
                return fm;
            }
            else if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsAudio).Contains<string>(fm.ExtensionLowerNoDot))
            //else if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsAudio).Contains<string>(fm.ExtensionLowerNoDot))
            //else if (fm.mi.Audio.Count > 0 || fm.mi.General.InternetMediaType.Contains("audio"))
            {
                fm.MediaType = MediaType.Audio;
                fm.FAIcon = MediaIcons.Audio;
                fm.BaseFileName = Path.GetFileNameWithoutExtension(fileName);
                return fm;
            }
            return null;
        }
    }
}
