using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.IO;

namespace DerOptimist
{
    /*
    class InputFile
    {
        public string FileName { get; set; }
        public string FileNameNoExt { get; set; }
        public string FullPath { get; set; }
        public string GeneratedOutputPath { get; set; }
        public bool OutputPathManualOverride { get; set; }
        public Guid id;
        public MediaType mediaType;

        public string BaseName { get; set; }
        public string AfterCounter { get; set; }
        public int First { get; }
        public int Last { get; }
        public int Digits { get; set; }
        string Extension;
        //public double FrameRate { get; set; }
        List<string> FileList { get; set; }

        public InputFile(string fullPath, MediaType newMediaType = MediaType.Unknown)
        {
            this.id = Guid.NewGuid();
            this.OutputPathManualOverride = false;
            this.mediaType = newMediaType;
            this.FileName = fullPath;
            this.FileNameNoExt = Path.GetFileNameWithoutExtension(fullPath);
            this.FullPath = Path.GetFullPath(fullPath);
        }

        public InputFile(string fullPath, string baseName, string first, string last, int digits, string afterCounter = "", List<string> fileList=null)
        {
            id = Guid.NewGuid();
            OutputPathManualOverride = false;
            mediaType = MediaType.ImageSequence;
            FileName = fullPath;
            FileNameNoExt = Path.GetFileNameWithoutExtension(fullPath);
            FullPath = Path.GetFullPath(fullPath);
            First = int.Parse(first);
            Last = int.Parse(last);
            Digits = digits;
            BaseName = baseName;
            AfterCounter = afterCounter;
            //FrameRate = Properties.Settings.Default.defaultFrameRate;
            Extension = Path.GetExtension(fullPath);
            FileList = fileList ?? new List<string>() { FullPath };
        }


        public Uri GetUri()
        {
            return new Uri(this.FullPath);
        }

        public static InputFile ParseInputFile(string file, List<Tuple<string, int>> ignoreList = null)
        {
            if (!File.Exists(file)) return null;
            if (ignoreList == null)
            {
                ignoreList = new List<Tuple<string, int>>();
            }
            string ext = Path.GetExtension(file);
            string extNoDot = ext.Remove(0, 1);
            if (Enum.GetNames(typeof(ExtensionsSkip)).Contains<string>(extNoDot))
            {
                return null;
            }
            if (Enum.GetNames(typeof(ExtensionsImages)).Contains<string>(extNoDot))
            {
                var fileDigits = Helpers.GetDigits(file);
                var ignoreTuple = new Tuple<string, int>(fileDigits.baseName, fileDigits.digits);
                if (ignoreList.Contains(ignoreTuple))
                {
                    return null;
                }
                if (fileDigits.digits == 0)
                {
                    // files that have no counter at all at its end are considered single images
                    return new InputFile(file, MediaType.Image);
                }
                var directoryName = Path.GetDirectoryName(file);
                var allFiles = Directory.EnumerateFiles(directoryName);
                //var searchPattern = new Regex(Regex.Escape(fileDigits.baseName) + @"\d{" + fileDigits.digits.ToString() + "}" + ext);
                var searchPattern = new Regex(fileDigits.pattern);
                // filter all files that have the same base name AND the same digit count
                var similarFiles = allFiles.Where(item => searchPattern.IsMatch(item));

                //var similarFiles = allFiles.Where(item =>
                //    item.StartsWith(fileDigits.Item1) &&
                //    StringHelper.GetDigits(item).Item2 == fileDigits.Item2
                //    );
                List<string> sortedList = similarFiles.OrderBy(q => q).ToList();
                
                if (sortedList.Count == 1)
                {
                    return new InputFile(file, MediaType.Image);
                }
                var firstItem = Helpers.GetDigits(sortedList.First());
                var lastItem = Helpers.GetDigits(sortedList.Last());
                return new InputFile(file, firstItem.baseName, firstItem.counterString, lastItem.counterString, firstItem.digits, firstItem.afterCounter, sortedList);
            }
            else if (Enum.GetNames(typeof(ExtensionsMovies)).Contains<string>(extNoDot))
            {
                return new InputFile(file, MediaType.Movie);
            }
            else if (Enum.GetNames(typeof(ExtensionsAudio)).Contains<string>(extNoDot))
            {
                return new InputFile(file, MediaType.Audio);
            }
            return null;
        }

        public long GetSize()
        {
            long sumbytes = 0;

            foreach (var f in FileList)
            {
                FileInfo fi = new FileInfo(f);
                sumbytes += fi.Length;
            }

            return sumbytes;
        }

        /// <summary>
        /// Returns an ffmpeg compatible string representing the image squence
        /// </summary>
        /// <returns></returns>
        public string GetSequenceString()
        {
            string counter = $"%0{this.Digits}d";
            string dir = Path.GetDirectoryName(this.FullPath);
            return Path.Combine(dir, this.BaseName + counter + this.AfterCounter + this.Extension);
        }

        /// <summary>
        /// Returns an ffme compatible Uri representing the image squence
        /// </summary>
        /// <returns></returns>
        public Uri GetSequenceUri()
        {
            return new Uri(GetSequenceString());
        }


        /// <summary>
        /// Assembles the filename from various parts, helper function for the various GetFilename functions
        /// </summary>
        /// <param name="frameNumber">Current frame in sequence (not the same necessarily the same as filename numbering)</param>
        /// <returns></returns>
        public string ConstructFileName(int frameNumber = 0)
        {
            int actualFrameNumber = this.First + frameNumber < this.Last ? this.First + frameNumber : this.Last;
            string counter = actualFrameNumber.ToString().PadLeft(this.Digits, '0');
            string dir = Path.GetDirectoryName(this.FullPath);
            return Path.Combine(dir, this.BaseName + counter + this.AfterCounter + this.Extension);
        }
    }
*/
}
