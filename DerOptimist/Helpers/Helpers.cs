using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
//using Xabe.FFmpeg;
using Unosquare.FFME;
using System.Diagnostics;
using MediaElement = Unosquare.FFME.MediaElement;
using System.Windows.Controls.Primitives;
using System.Windows.Threading;
using System.IO;
using System.Management;
using Path = System.IO.Path;
using System.Text.RegularExpressions;

namespace DerOptimist
{
    static class Helpers
    {
        public static readonly string HelpText = @"
Shortcuts
---------
F1          This help overlay
F2          MediaInfo overlay
F3          Toggle split view

1           Fit image to window
2           Scale x0.5
3           Scale x1.0
4           Scale x2.0

R           Render preview
Shift-R     Render final

Left        Step one frame back
Shift-Left  Step one second back
Left        Step one frame forward
Shift-Left  Step one second forward

Mousewheel  Zoom in/out
Left-Drag   Jog forward/backward
Middle-Drag Pan

Space       Play left
Shift-Space Play right

I           Set range in-point
O           Set range out-point
";

        public static void RepeatAction(int repeatCount, Action action)
        {
            for (int i = 0; i < repeatCount; i++)
                action();
        }

        public sealed class FilteredEventHandler
        {
            private readonly Func<bool> supressEvent;
            private readonly EventHandler realEvent;

            public FilteredEventHandler(Func<bool> supressEvent, EventHandler eventToRaise)
            {
                this.supressEvent = supressEvent;
                this.realEvent = eventToRaise;
            }

            //Checks the "supress" flag and either call the real event handler, or skip it
            public void FakeEventHandler(object sender, EventArgs e)
            {
                if (!this.supressEvent())
                {
                    this.realEvent(sender, e);
                }
            }
        }

        public static List<string> GetTrimmedSplitStringList(string sourceString)
        {
            return sourceString.Split(',').Select(s => s.Trim()).ToList();
        }

        private static readonly Regex _regex = new Regex("[^0-9.-]+"); //regex that matches disallowed text
        public static bool IsTextAllowed(string text)
        {
            return !_regex.IsMatch(text);
        }

        public static string GetUNCPath(string path)
        {
            if (path.StartsWith(@"\\"))
            {
                return path;
            }

            ManagementObject mo = new ManagementObject();
            mo.Path = new ManagementPath(String.Format("Win32_LogicalDisk='{0}'", path));

            // DriveType 4 = Network Drive
            if (Convert.ToUInt32(mo["DriveType"]) == 4)
            {
                return Convert.ToString(mo["ProviderName"]);
            }
            else
            {
                return path;
            }
        }

        public static Thumb GetThumb(Slider slider)
        {
            var track = slider.Template.FindName("PART_Track", slider) as Track;
            return track == null ? null : track.Thumb;
        }

        public static double? ConvertAspectRatioStringToDouble(string aspectRatio, char separator=':')
        {
            var split = aspectRatio.Split(separator);
            if (split.Length == 2)
            {
                double w, h;
                if (double.TryParse(split[0], out w) && double.TryParse(split[1], out h))
                {
                    return w / h;
                }
            }
            return null;
        }

        // Gets the absolute mouse position, relative to screen
        public static Point GetMousePos(Control ctrl)
        {
            return ctrl.PointToScreen(Mouse.GetPosition(ctrl));
        }
        
        public static T GetVisualParent<T>(DependencyObject d) where T:class
        {
            while (d != null && !(d is T))
            {
                d = VisualTreeHelper.GetParent(d);
            }
            return d as T;
        }

        //public static MediaType MediaTypeFromFileName(string fullPath)
        //{
        //    string extNoDot = Path.GetExtension(fullPath).Remove(0, 1);
        //    if (Enum.GetNames(typeof(ExtensionsImages)).Contains<string>(extNoDot))
        //    {
        //        return MediaType.Image;
        //    }
        //    else if (Enum.GetNames(typeof(ExtensionsMovies)).Contains<string>(extNoDot))
        //    {
        //        return MediaType.Movie;
        //    }
        //    else if (Enum.GetNames(typeof(ExtensionsAudio)).Contains<string>(extNoDot))
        //    {
        //        return MediaType.Audio;
        //    }
        //    return MediaType.Unknown;
        //}

        public static MediaType MediaTypeFromFileName(string fullPath)
        {
            string extension = Path.GetExtension(fullPath);
            if (extension == null || extension.Length == 0) return MediaType.Unknown;
            string extNoDot = extension.Remove(0, 1);
            var mi = new MediaInfoDotNet.MediaFile(fullPath);
            //if (mi.Image.Count > 0 || mi.General.InternetMediaType.Contains("image"))
            if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsImage).Contains<string>(extNoDot))
            //if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsImages).Contains<string>(extNoDot))
            {
                return MediaType.Image;
            }
            else if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsVideo).Contains<string>(extNoDot))
            //else if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsVideo).Contains<string>(extNoDot))
            //if (mi.Video.Count > 0 || mi.General.InternetMediaType.Contains("video"))
            {
                return MediaType.Movie;
            }
            else if (Helpers.GetTrimmedSplitStringList(Properties.Settings.Default.extensionsAudio).Contains<string>(extNoDot))
            //else if (Helpers.GetTrimmedSplitStringList(MainWindow.ExtensionsAudio).Contains<string>(extNoDot))
            //if (mi.Audio.Count > 0 || mi.General.InternetMediaType.Contains("audio"))
            {
                return MediaType.Audio;
            }
            return MediaType.Unknown;
        }

        /// <summary>
        /// Receives string and returns the string with its letters reversed.
        /// </summary>
        public static string ReverseString(string s)
        {
            char[] arr = s.ToCharArray();
            Array.Reverse(arr);
            return new string(arr);
        }

        /// <summary>
        /// Parses a filename string and returns a Tuple of base name (str), digit count (int) and the counter string itself (str).
        /// </summary>
        /// <param name="fullPath"></param>
        /// <returns>Tuple of string baseName, int digits, string counterString, string pattern, string afterCounter</returns>
        public static (string baseName, int digits, string counterString, string pattern, string afterCounter) GetDigits(string fullPath)
        {
            var digitRegex = new Regex(@"\d+");
            var extension = Path.GetExtension(fullPath);
            var fnNoExt = Path.GetFileNameWithoutExtension(fullPath);
            var matches = digitRegex.Matches(fnNoExt);
            if (matches.Count > 0)
            {
                var lastMatch = matches[matches.Count - 1];
                var baseFileName = digitRegex.Replace(fnNoExt, "", 1, lastMatch.Index);
                var afterMatch = fnNoExt.Substring(lastMatch.Index + lastMatch.Length);
                baseFileName = baseFileName.Substring(0, lastMatch.Index);
                var baseName = Path.Combine(Path.GetDirectoryName(fullPath), baseFileName);

                var patternString = digitRegex.Replace(Regex.Escape(fnNoExt), @"\d{" + lastMatch.Length.ToString() + "}", 1, lastMatch.Index) + extension;
                return (baseName: baseName, digits: lastMatch.Length, counterString: lastMatch.Value, pattern: patternString, afterCounter: afterMatch);
            }
            else
            {
                return (baseName:fullPath, digits:0, counterString:"", pattern:"", afterCounter:"");
            }
        }

        public static List<InputFileGroup> ConsolidateSequences(string[] files)
        {
            List<InputFileGroup> groups = new List<InputFileGroup>();
            foreach (var f in files)
            {
                string ext = Path.GetExtension(f);
                string extNoDot = ext.Remove(0, 1);
                var mt = MediaTypeFromFileName(f);
                if (mt == MediaType.Image)
                {
                    var res = GetDigits(f);
                    var fg = (groups.Where<InputFileGroup>(x => x.BaseName == res.baseName).DefaultIfEmpty(new InputFileGroup())).First();
                    fg.Files.Add(f);
                    if (fg.Files.Count == 1)
                    {
                        if (int.TryParse(res.counterString, out fg.FirstDigit) == false) fg.FirstDigit = 0;
                        fg.BaseName = res.baseName;
                        fg.MediaT = MediaType.Image;
                        groups.Add(fg);
                    }
                    else
                    {
                        fg.MediaT = MediaType.ImageSequence;
                    }
                }
                else
                {
                    var fg = new InputFileGroup(f);
                    fg.MediaT = mt;
                    groups.Add(fg);
                }
            }
            return groups;
        }

        public static int Clamp(int value, int min, int max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static double Clamp(double value, double min, double max)
        {
            return (value < min) ? min : (value > max) ? max : value;
        }

        public static Point Clamp(Point value, double min, double max)
        {
            return new Point(((value.X < min) ? min : (value.X > max) ? max : value.X), ((value.Y < min) ? min : (value.Y > max) ? max : value.Y));
        }

        public static TranslateTransform GetTranslateTransform(MediaElement m)
        {
            if (m.RenderTransform is TransformGroup renderTransformsA)
            {
                return renderTransformsA.Children[0] as TranslateTransform;
            }
            else
            {
                renderTransformsA = new TransformGroup();
                var translateTransformA = new TranslateTransform(0, 0);
                renderTransformsA.Children.Add(translateTransformA);
                m.RenderTransform = renderTransformsA;
                return translateTransformA;
            }
        }

        public static void GetTransformsFromMediaElement(ref MediaElement m, ref (TranslateTransform Translate,ScaleTransform Scale,RotateTransform Rotate) t)
        {
            if (m.RenderTransform is TransformGroup renderTransformsA)
            {
                t.Translate = renderTransformsA.Children[0] as TranslateTransform;
            }
            else
            {
                renderTransformsA = new TransformGroup();
                var translateTransformA = new TranslateTransform(0, 0);
                renderTransformsA.Children.Add(translateTransformA);
                m.RenderTransform = renderTransformsA;
                t.Translate =  translateTransformA;
            }

            if (m.LayoutTransform is TransformGroup layout)
            {
                t.Scale = layout.Children[0] as ScaleTransform;
                t.Rotate = layout.Children[1] as RotateTransform;
            }
            else
            {
                layout = new TransformGroup();
                t.Scale = new ScaleTransform(1, 1);
                t.Rotate = new RotateTransform(0, 0.5, 0.5);
                layout.Children.Add(t.Scale);
                layout.Children.Add(t.Rotate);
                m.LayoutTransform = layout;
            }
        }
    }
}

