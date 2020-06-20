using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using Path = System.IO.Path;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Unosquare.FFME;
using System.Diagnostics;
using System.Windows.Threading;
using System.Windows.Controls.Primitives;
using MediaElement = Unosquare.FFME.MediaElement;
using System.Collections.ObjectModel;
using MahApps.Metro.Controls;
using System.IO;
using FFmpeg.NET;
using FFmpeg.NET.Enums;
using System.Reflection;
using System.Threading;
using Microsoft.Win32;
using System.Globalization;
using Newtonsoft.Json;
using System.Runtime.InteropServices;
using Unosquare.FFME.Common;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow
    {
        public string VERSION { get; } = "v0.1.83";
        public int BUILD = 10021;
        public RenderSettingsCollection RenderHistory;
        public RenderSettingsCollection RenderQueue;
        public Preset RenderPresets;
        public RenderSettingsItem CurrentSettings { get; set; }
        public RenderSettingsItem TempSettings { get; set; }

        FileMeta InputA, InputB;
        FileInfo DrawFont;
        string FontPath;
        private WindowPreferences WinPreferences;
        private WindowRenderHistory WinRenderHistory;
        private WindowRenderQueue WinRenderQueue;
        private WindowPresets WinPresets;
        private SequenceDialog WinSequenceDialog;
        private bool DoPlayRanged { get; set; } = false;
        private Engine ffmpeg;
        private CancellationTokenSource RenderCancel;
        private (TranslateTransform Translate,ScaleTransform Scale,RotateTransform Rotate) TransformsA, TransformsB;
        private Point TranslateOffsetA { get; set; }
        private Point TranslateOffsetB { get; set; }
        private double CurrentZoomLevel {
            get {
                return TransformsA.Scale.ScaleX;
            }
        }
        private bool QueueIsRendering { get; set; } = false;
        private FrameRates.FrameRateEntry CurrentFrameRateEntry;
        private double ZoomLevelFit
        {
            get { return Math.Min(BorderA.ActualWidth / MediaA.NaturalVideoWidth, BorderA.ActualHeight / MediaA.NaturalVideoHeight); }
        }
        private double ZoomLevelTarget { get; set; } = 1.0;
        private double ZoomLevelOffset { get; set; } = 0.0;
        private double ZoomLevelOffsetB { get; set; } = 0.0;
        private Point TranslateDragStartPos { get; set; }
        private Point MouseDragStartPos { get; set; }

        private bool dontUpdateAspectRatio = false;
        private bool _resetZoom = false;
        private bool wasPlayingBeforeDraggingA { get; set; } = false;
        private bool wasPlayingBeforeDraggingB { get; set; } = false;
        private bool showRightColumn { get; set; } = true;
        private double ZoomLevelFitA {
            get { return Helpers.Clamp(Math.Min(BorderA.ActualWidth / MediaA.NaturalVideoWidth, BorderA.ActualHeight / MediaA.NaturalVideoHeight) + ZoomLevelOffset, 0.1, 5); }
        }
        private Cursor _originalCursor;

        [DllImport("user32.dll")]
        static extern bool SetCursorPos(int xPos, int yPos);
        static bool SetCursorPos(Point pos) { return SetCursorPos((int)pos.X, (int)pos.Y); }

        public MainWindow()
        {
            try
            {
                InitAndReadPreferences();
                InitializeValues();
                InitializeComponent();
                SetupEventsSplitView();
                ReadPresets();
                this.Closing += MainWindow_Closing;
                AppDomain.CurrentDomain.ProcessExit += CurrentDomain_ProcessExit;
                ToggleShowLog(ToggleShowLog_Value);
                ComboPixFmt.Loaded += (o, e) =>
                {
                    UpdatePixFmts();
                };
                UpdateMediaSources();
                ReadPresetsMenuItems(Menu_PresetsRoot);
                _originalCursor = Cursor;
                HideRightPlayer();
                MaxConcurrentEncoding = 2;
                ButtonSendToQueue.IsEnabled = false;
                ButtonRenderFinal.IsEnabled = false;
                ButtonRenderPreview.IsEnabled = false;
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
                throw;
            }
        }

        
        private void InitAndReadPreferences()
        {
            var AssemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            var AssemblyPath = Path.GetDirectoryName(AssemblyLocation);
            if (Properties.Settings.Default.ffmpegBinaryPath == "") Properties.Settings.Default.ffmpegBinaryPath = Path.Combine(AssemblyPath, "ffmpeg\\ffmpeg.exe");
            if (Properties.Settings.Default.PresetFolderPath == "") Properties.Settings.Default.PresetFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DerOptimist\\Presets");
            if (!Directory.Exists(Properties.Settings.Default.PresetFolderPath)) Directory.CreateDirectory(Properties.Settings.Default.PresetFolderPath);
            if (Properties.Settings.Default.defaultEncoderVideo == "") Properties.Settings.Default.defaultEncoderVideo = VideoCodec.Default;
            if (Properties.Settings.Default.defaultEncoderAudio == "") Properties.Settings.Default.defaultEncoderAudio = AudioCodec.Default;
            if (Properties.Settings.Default.defaultOutputPath == "") Properties.Settings.Default.defaultOutputPath = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            if (Properties.Settings.Default.defaultPreviewPath == "") Properties.Settings.Default.defaultPreviewPath = Path.GetTempPath();
            if (Properties.Settings.Default.RecentFiles == null) Properties.Settings.Default.RecentFiles = new System.Collections.Specialized.StringCollection();
            if (Properties.Settings.Default.Version < 10018)
            {
                // Reset some preferences in older builds
                Properties.Settings.Default.PresetFolderPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DerOptimist\\Presets");
                if (!Directory.Exists(Properties.Settings.Default.PresetFolderPath)) Directory.CreateDirectory(Properties.Settings.Default.PresetFolderPath);
            }
            if (Properties.Settings.Default.Version < BUILD)
            {
                Properties.Settings.Default.Version = BUILD;
            }
            Properties.Settings.Default.Save();
        }

        private void InitializeValues()
        {
            DataContext = this;
            RenderHistory = new RenderSettingsCollection();
            RenderQueue = new RenderSettingsCollection();
            RenderPresets = new Preset();

            var AssemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            var AssemblyPath = Path.GetDirectoryName(AssemblyLocation);
            FontPath = Path.Combine(AssemblyPath, "fonts");
            DrawFont = new FileInfo(Path.Combine(FontPath, "FiraCode-Light.ttf"));

            TranslateOffsetA = new Point();
            TranslateOffsetB = new Point();

            TryFfmpeg();

            ffmpeg = new Engine(Properties.Settings.Default.ffmpegBinaryPath);
            ffmpeg.Progress += Ffmpeg_OnProgress;
            ffmpeg.Data += Ffmpeg_OnData;
            ffmpeg.Error += Ffmpeg_OnError;
            ffmpeg.Complete += Ffmpeg_OnComplete;

            Environment.SetEnvironmentVariable("FC_CONFIG_DIR", FontPath);
            Environment.SetEnvironmentVariable("FONTCONFIG_FILE", "fonts.conf");
            Environment.SetEnvironmentVariable("FONTCONFIG_PATH", FontPath);
        }

        public Preset ReadPresets()
        {
            Preset root = new Preset();
            if (!Directory.Exists(Properties.Settings.Default.PresetFolderPath)) return root;

            var dirs = Directory.EnumerateDirectories(Properties.Settings.Default.PresetFolderPath);

            Preset parent = root;
            foreach (var f in Directory.EnumerateFiles(Properties.Settings.Default.PresetFolderPath, "*.json")) {
                var p = PresetFromFile(f);
                if (p != null)
                {
                    parent.Items.Add(p);
                }
            }

            foreach (var dir in dirs)
            {
                var jsonFiles = Directory.EnumerateFiles(dir, "*.json");
                var p = PresetFromFile(dir);
                if (p != null && jsonFiles.Count()>0)
                {
                    root.Items.Add(p);
                    parent = p;
                }
                else
                {
                    continue;
                }
                foreach (var f in jsonFiles)
                {
                    p = PresetFromFile(f);
                    if (p != null)
                    {
                        parent.Items.Add(p);
                    }
                }
            }
            return root;
        }

        public MenuItem ReadPresetsMenuItems(MenuItem root)
        {
            if (!Directory.Exists(Properties.Settings.Default.PresetFolderPath)) return root;

            root.Items.Clear();
            var dirs = Directory.EnumerateDirectories(Properties.Settings.Default.PresetFolderPath);

            MenuItem parent = root;
            foreach (var f in Directory.EnumerateFiles(Properties.Settings.Default.PresetFolderPath,"*.json"))
            {
                var p = MenuItemFromPresetFile(f);
                if (p != null)
                {
                    parent.Items.Add(p);
                }
            }

            foreach (var dir in dirs)
            {
                var p = MenuItemFromPresetFile(dir);
                if (p != null)
                {
                    root.Items.Add(p);
                    parent = p;
                }
                else
                {
                    continue;
                }
                foreach (var f in Directory.EnumerateFiles(dir))
                {
                    p = MenuItemFromPresetFile(f);
                    if (p != null)
                    {
                        parent.Items.Add(p);
                    }
                }
            }
            return root;
        }

        private Preset PresetFromFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return new Preset()
                {
                    //Name = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar).Last(),
                    Name = path.Split(Path.DirectorySeparatorChar).Last(),
                    IsFolder = true,
                    Settings = null,
                    PresetFileName = null,
                };
            }
            else
            {
                RenderSettingsItem rs = RenderSettingsItem.FromJsonFile(path, sanitize: true);
                if (rs == null) return null;
                return new Preset()
                {
                    Name = rs.Name,
                    IsFolder = attr.HasFlag(FileAttributes.Directory),
                    PresetFileName = path,
                    Settings = rs
                };
            }
        }

        private MenuItem MenuItemFromPresetFile(string path)
        {
            FileAttributes attr = File.GetAttributes(path);

            if (attr.HasFlag(FileAttributes.Directory))
            {
                return new MenuItem()
                {
                    //Header = Path.GetDirectoryName(path).Split(Path.DirectorySeparatorChar).Last(),
                    Header = path.Split(Path.DirectorySeparatorChar).Last(),
                    Tag = null
                };
            }
            else
            {
                RenderSettingsItem rs = RenderSettingsItem.FromJsonFile(path, sanitize: true);
                if (rs == null) return null;
                var m = new MenuItem()
                {
                    Header = rs.Name,
                    Tag = rs,
                };
                m.Click += Menu_LoadPresetsFromTag;
                return m;
            }
        }

        private void TryFfmpeg()
        {
            var AssemblyLocation = System.Reflection.Assembly.GetEntryAssembly().Location;
            var AssemblyPath = Path.GetDirectoryName(AssemblyLocation);

            try
            {
                Unosquare.FFME.Library.FFmpegDirectory = Path.GetDirectoryName(Properties.Settings.Default.ffmpegBinaryPath);
                Unosquare.FFME.Library.LoadFFmpeg();
            }
            catch (System.IO.FileNotFoundException ex)
            {
                Debug.WriteLine(ex.Message);
                var res = MessageBox.Show("Could not find ffmpeg.exe. Press YES to try the default location, press NO to select your own file or press CANCEL to close.", "Error opening ffmpeg binary", MessageBoxButton.YesNoCancel);
                if (res == MessageBoxResult.Yes)
                {
                    Properties.Settings.Default.ffmpegBinaryPath = Path.Combine(AssemblyPath, "ffmpeg", "ffmpeg.exe");
                    Properties.Settings.Default.Save();
                    TryFfmpeg();
                }
                else if (res == MessageBoxResult.No)
                {
                    OpenFileDialog openFileDialog = new OpenFileDialog();
                    openFileDialog.Filter = "ffmpeg.exe (*.exe)|*.exe";
                    if (openFileDialog.ShowDialog() == true)
                    {
                        Properties.Settings.Default.ffmpegBinaryPath = openFileDialog.FileName;
                        Properties.Settings.Default.Save();
                        TryFfmpeg();
                    }
                }
                else
                {
                    System.Windows.Application.Current.Shutdown();
                }
            }

        }

        private void Ffmpeg_OnComplete(object sender, FFmpeg.NET.Events.ConversionCompleteEventArgs e)
        {
            if (e.Output == null)
            {
                return;
            }
            if (e.Options != null && e.Options.renderSettingsItem != null)
            {
                e.Options.renderSettingsItem.Progress = 100.0;
                e.Options.renderSettingsItem.Status = RenderQueueItemStatus.Finished;
                if (e.Options.renderSettingsItem.Queued == true)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => WinRenderQueue.RefreshItems()));
                }
                else
                {
                    Debug.WriteLine("Completed conversion from {0} to {1}", e.Input?.FileInfo?.FullName ?? "n/a", e.Output?.FileInfo?.FullName ?? "n/a");
                    InputB = FileMeta.Create(e.Output.FileInfo.FullName);
                    if (InputB.mi == null) InputB.mi = new MediaInfoDotNet.MediaFile(e.Output.FileInfo.FullName);
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ProgressFF.Value = 0));
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ButtonStopRender.IsEnabled = false));
                    WriteToLog(e.Messages);
                    if (InputB != null)
                    {
                        UpdateMediaSources(onlyRightSide: true);
                    }
                }
            }
            if (QueueIsRendering)
            {
                RenderAllQueued();
            }
        }

        private void Ffmpeg_OnError(object sender, FFmpeg.NET.Events.ConversionErrorEventArgs e)
        {
            string msg = String.Format("[{0} => {1}]: Error: {2}\n{3}", e.Input?.FileInfo?.Name ?? "n/a", e.Output?.FileInfo?.Name ?? "n/a", e.Exception.ExitCode, e.Exception.InnerException);
            Debug.WriteLine(msg);
            WriteToLog(e.Messages);
            if (e.Options != null && e.Options.renderSettingsItem != null)
            {
                e.Options.renderSettingsItem.Status = RenderQueueItemStatus.Error;
                e.Options.renderSettingsItem.Msg = msg;
                if (e.Options.renderSettingsItem.Queued == true)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => WinRenderQueue.RefreshItems()));
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ProgressFF.Value = 0));
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ButtonStopRender.IsEnabled = false));
                }
            }
            if (QueueIsRendering)
            {
                RenderAllQueued();
            }
        }

        private void Ffmpeg_OnData(object sender, FFmpeg.NET.Events.ConversionDataEventArgs e)
        {
            Debug.WriteLine("[{0} => {1}]: {2}", e.Input?.FileInfo?.Name ?? "n/a", e.Output?.FileInfo?.Name ?? "n/a", e.Data ?? "n/a");
            //WriteToLog(String.Format("[{0} => {1}]: {2}", e.Input?.FileInfo?.Name ?? "n/a", e.Output?.FileInfo?.Name ?? "n/a", e.Data ?? "n/a"));
        }

        private void Ffmpeg_OnProgress(object sender, FFmpeg.NET.Events.ConversionProgressEventArgs e)
        {
            double done = Helpers.Clamp((e.ProcessedDuration.TotalSeconds / TempSettings.Duration.TotalSeconds) * 100.0, 0, 100);
            if (e.Options != null && e.Options.renderSettingsItem != null)
            {
                e.Options.renderSettingsItem.Progress = done;
                e.Options.renderSettingsItem.Status = RenderQueueItemStatus.Encoding;
                if (e.Options.renderSettingsItem.Queued == true)
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => WinRenderQueue.RefreshItems()));
                    Debug.WriteLine($"Cancel: {e.Options.renderSettingsItem.cancellationTokenSource.IsCancellationRequested}");
                    Debug.WriteLine($"{e.ProcessedDuration.TotalSeconds} / {e.Options.renderSettingsItem.Duration.TotalSeconds} * 100 = {done}");
                }
                else
                {
                    Dispatcher.BeginInvoke(DispatcherPriority.Input, new Action(() => ProgressFF.Value = done));
                    //Debug.WriteLine($"Cancel: {RenderCancel.IsCancellationRequested}");
                    Debug.WriteLine($"Cancel: {e.Options.renderSettingsItem.cancellationTokenSource.IsCancellationRequested}");
                    //Debug.WriteLine($"{e.ProcessedDuration.TotalSeconds} / {TempSettings.Duration.TotalSeconds} * 100 = {done}");
                    Debug.WriteLine($"{e.ProcessedDuration.TotalSeconds} / {e.Options.renderSettingsItem.Duration.TotalSeconds} * 100 = {done}");
                }
            }
            Debug.WriteLine("[{0} => {1}]", e.Input?.FileInfo?.Name ?? "n/a", e.Output?.FileInfo?.Name ?? "n/a");
            Debug.WriteLine("Bitrate: {0}", e.Bitrate);
            Debug.WriteLine("Fps: {0}", e.Fps);
            Debug.WriteLine("Frame: {0}", e.Frame);
            Debug.WriteLine("ProcessedDuration: {0}", e.ProcessedDuration);
            Debug.WriteLine("Size: {0} kb", e.SizeKb);
            Debug.WriteLine("TotalDuration: {0}\n", e.TotalDuration);
        }

        private void WriteToLog(string s, bool newline=true)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                TheLog += $"{s}" + (newline ? "\n" : "");
                if (CheckLogFollow.IsChecked == true) LogOutput.ScrollToEnd();
            }));
            TempSettings.Log += $"{s}" + (newline ? "\n" : "");
        }

        private void WriteToLog(List<string> messages, bool newline = true)
        {
            Dispatcher.BeginInvoke(DispatcherPriority.Background, new Action(() =>
            {
                string tmp = "";
                foreach (var s in messages)
                {
                    tmp += $"{s}" + (newline ? "\n" : "");
                }
                TheLog += tmp;
                TempSettings.Log += tmp;
                if (CheckLogFollow.IsChecked == true) LogOutput.ScrollToEnd();
            }));
            string tmp2 = "";
            foreach (var s in messages)
            {
                tmp2 += $"{s}" + (newline ? "\n" : "");
            }
            TempSettings.Log += tmp2;
        }

        /// <summary>
        /// Delete all the temporary preview files that were created
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void CurrentDomain_ProcessExit(object sender, EventArgs e)
        {
            if (!Properties.Settings.Default.KeepPreviewFiles)
            {
                foreach (var entry in RenderHistory)
                {
                    if (entry.IsPreview == true)
                        DeletePreviewFile(entry.OutputPath);
                }
            }
        }

        public void DeletePreviewFromHistory(Guid key)
        {
            var path = RenderHistory[key].OutputPath;
            if (!Properties.Settings.Default.KeepPreviewFilesHistoryDelete)
            {
                DeletePreviewFile(path);
                DeletePreviewFile(path+".six");
            }
            RenderHistory.Remove(key);
        }

        public void DeleteQueueItemFromHistory(Guid key)
        {
            RenderQueue.Remove(key);
        }

        private void DeletePreviewFile(string path)
        {
            if (path != null && path != "" && File.Exists(path))
            {
                try
                {
                    File.Delete(path);
                }
                catch (Exception)
                {
                    Debug.WriteLine($"Could not delete {path}");
                }
            }
        }

        private void MainWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            MediaA?.Stop();
            MediaB?.Stop();
            MediaA?.Close();
            MediaB?.Close();
            WinRenderQueue?.Close();
            WinSequenceDialog?.Close();
            WinPreferences?.Close();
            WinRenderHistory?.Close();
            //MediaA?.Dispose();
            //MediaB?.Dispose();
        }

        private async void UpdateMediaSources(bool onlyRightSide=false, bool onlyLeftSide=false, bool forceReload=false)
        {
            if (MediaA == null || InputA == null) return;
            if (MediaB == null || (InputB == null && onlyRightSide == true)) return;

            var originalA = MediaA?.Source?.LocalPath ?? "";
            var originalB = MediaB?.Source?.LocalPath ?? "";

            if (!forceReload && (originalA == InputA.GetUri().LocalPath) && (originalB == InputB.GetUri().LocalPath))
                return;
            else if (InputA != null && (!forceReload && originalA == InputA.GetUri().LocalPath))
                onlyRightSide = true;
            else if (InputB != null && (!forceReload && originalB == InputB.GetUri().LocalPath))
                onlyLeftSide = true;

            if (!onlyRightSide)
            {
                if (InputA != null)
                {
                    if (InputA.MediaType == MediaType.ImageSequence)
                    {
                        //MediaA.Source = InputA.GetSequenceUri();
                        await MediaA.Open(InputA.GetSequenceUri());
                        AudioOnlyLabelA.Visibility = Visibility.Collapsed;
                    }
                    else if (InputA.MediaType == MediaType.Audio)
                    {
                        //MediaA.Source = InputA.GetUri();
                        await MediaA.Open(InputA.GetUri());
                        AudioOnlyLabelA.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //MediaA.Source = InputA.GetUri();
                        await MediaA.Open(InputA.GetUri());
                        AudioOnlyLabelA.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    await MediaA.Close();
                    //MediaA.Source = null;
                }
            }

            if (!onlyLeftSide)
            {
                if (InputB != null)
                {
                    //MediaB.Source = mediaFileB;
                    await MediaB.Close();
                    //MediaB.Source = null;
                    if (InputB.MediaType == MediaType.ImageSequence)
                    {
                        //MediaB.Source = InputB.GetSequenceUri();
                        await MediaB.Open(InputB.GetSequenceUri());
                        AudioOnlyLabelB.Visibility = Visibility.Collapsed;
                    }
                    else if (InputB.MediaType == MediaType.Audio)
                    {
                        //MediaB.Source = InputB.GetUri();
                        await MediaB.Open(InputB.GetUri());
                        AudioOnlyLabelB.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        //MediaB.Source = InputB.GetUri();
                        await MediaB.Open(InputB.GetUri());
                        await MediaB.Open(InputB.GetUri());
                        AudioOnlyLabelB.Visibility = Visibility.Collapsed;
                    }
                }
                else
                {
                    //MediaB.Source = null;
                    await MediaB.Close();
                }
            }
        }


        private void SetupEventsSplitView()
        {
            MediaElement a = MediaA;
            MediaElement b = MediaB;
            Thumb thumbA = Helpers.GetThumb(SliderPlayA) ?? null;
            Thumb thumbB = Helpers.GetThumb(SliderPlayB) ?? null;

            a.MediaReady += MediaA_SourceUpdated;
            b.MediaReady += MediaB_SourceUpdated;
           
            // ***
            // Transforms
            // ***
            Helpers.GetTransformsFromMediaElement(ref MediaA, ref TransformsA);
            Helpers.GetTransformsFromMediaElement(ref MediaB, ref TransformsB);

            BorderA.MouseWheel += Border_MouseWheel;
            BorderB.MouseWheel += Border_MouseWheel;

            ButtonPlayA.Click += ButtonPlay_Click;
            ButtonPlayB.Click += ButtonPlay_Click;

            ButtonSetInPoint.Click += (o, e) =>
            {
                if (ToggleManualRangeMode_Value == false) { ToggleManualRangeMode_Value = true; }
                if (MediaA.Position.TotalSeconds >= RangeSliderPlayA_RangeUpperValue)
                {
                    RangeSliderPlayA_RangeUpperValue = MediaA.Position.TotalSeconds + 1;
                }
                RangeSliderPlayA_RangeLowerValue = MediaA.Position.TotalSeconds;
                
            };

            ButtonSetOutPoint.Click += (o, e) =>
            {
                if (ToggleManualRangeMode_Value == false) { ToggleManualRangeMode_Value = true; }
                if (MediaA.Position.TotalSeconds <= RangeSliderPlayA_RangeLowerValue)
                {
                    RangeSliderPlayA_RangeLowerValue = MediaA.Position.TotalSeconds - 1;
                }
                RangeSliderPlayA_RangeUpperValue = MediaA.Position.TotalSeconds;
            };

            a.MediaEnded += async (o, e) =>
            {
                if (ToggleLoopedPlayback_Value)
                {
                    if (a.IsPlaying)
                    {
                        await a.Seek(TimeSpan.Zero);
                        await a.Play();
                        ButtonPlayA.Content = MediaIcons.Pause;
                    }
                }
                else
                {
                    if (a.IsPlaying)
                    {
                        await a.Pause();
                        ButtonPlayA.Content = MediaIcons.Play;
                    }
                }
            };
            b.MediaEnded += async (o, e) =>
            {
                if (ToggleLoopedPlayback_Value)
                {
                    await b.Seek(TimeSpan.Zero);
                    await b.Play();
                    ButtonPlayB.Content = MediaIcons.Pause;
                }
                else
                {
                    await b.Pause();
                    ButtonPlayB.Content = MediaIcons.Play;
                }
            };

            a.PositionChanged += MediaA_PositionChanged;
            b.PositionChanged += MediaB_PositionChanged;

            //a.MediaOpening += A_MediaOpening;
            //b.MediaOpening += A_MediaOpening;

            a.MediaInitializing += A_MediaInitializing;
            b.MediaInitializing += B_MediaInitializing;

            a.MediaOpened += (o, e) => {
                ResizeVideoWidth.Text = MediaA.NaturalVideoWidth.ToString();
                ResizeVideoHeight.Text = MediaA.NaturalVideoHeight.ToString();
                CurrentSettings.Width = MediaA.NaturalVideoWidth;
                CurrentSettings.Height = MediaA.NaturalVideoHeight;
            };

            BorderA.MouseDown += (o, e) =>
            {
                MouseDragStartPos = Helpers.GetMousePos(this);
                if (Mouse.MiddleButton == MouseButtonState.Pressed)
                {
                    TranslateDragStartPos = TranslateOffsetA;
                }
                else
                {
                    if (InputA == null)
                    {
                        if (WinSequenceDialog == null || WinSequenceDialog.IsLoaded == false)
                            OpenFile();
                        return;
                    }
                }
            };
            BorderB.MouseDown += (o, e) =>
            {
                MouseDragStartPos = Helpers.GetMousePos(this);
                if (Mouse.MiddleButton == MouseButtonState.Pressed)
                {
                    TranslateDragStartPos = TranslateOffsetB;
                }
            };

            BorderA.MouseMove += Border_MouseMove;
            BorderB.MouseMove += Border_MouseMove;

            FFMainWindow.PreviewMouseUp += (o, e) =>
            {
                Cursor = _originalCursor;
            };

            if (thumbA != null)
            {
                thumbA.DragStarted += async (o, e) =>
                {
                    if (a.IsPlaying)
                    {
                        wasPlayingBeforeDraggingA = true;
                    }
                    await a.Pause();
                };

                thumbA.DragDelta += (o, e) =>
                {
                };

                thumbA.DragCompleted += async (o, e) =>
                {
                    if (wasPlayingBeforeDraggingA)
                        await a.Play();
                };
            }

            if (thumbB != null)
            {
                thumbB.DragStarted += async (o, e) =>
                {
                    if (b.IsPlaying)
                    {
                        wasPlayingBeforeDraggingB = true;
                    }
                    await b.Pause();
                };

                thumbB.DragDelta += (o, e) =>
                {
                };

                thumbB.DragCompleted += async (o, e) =>
                {
                    if (wasPlayingBeforeDraggingB)
                        await b.Play();
                };
            }
        }

        private void A_MediaOpening(object sender, MediaOpeningEventArgs e)
        {
            // Get the local file path from the URL (if possible)
            var mediaFilePath = string.Empty;
            try
            {
                var url = new Uri(e.Info.MediaSource);
                mediaFilePath = url.IsFile || url.IsUnc ? Path.GetFullPath(url.LocalPath) : string.Empty;
            }
            catch { /* Ignore Exceptions */ }

            if (e.Options.VideoStream is StreamInfo videoStream)
            {
                // If we have a valid seek index let's use it!
                if (string.IsNullOrWhiteSpace(mediaFilePath) == false)
                {
                    try
                    {
                        // Try to Create or Load a Seek Index
                        var durationSeconds = e.Info.Duration.TotalSeconds > 0 ? e.Info.Duration.TotalSeconds : 0;
                        var seekIndex = LoadOrCreateVideoSeekIndex(mediaFilePath, videoStream.StreamIndex, durationSeconds);

                        // Make sure the seek index belongs to the media file path
                        if (seekIndex != null &&
                            string.IsNullOrWhiteSpace(seekIndex.MediaSource) == false &&
                            seekIndex.MediaSource.Equals(mediaFilePath) &&
                            seekIndex.StreamIndex == videoStream.StreamIndex)
                        {
                            // Set the index on the options object.
                            e.Options.VideoSeekIndex = seekIndex;
                        }
                    }
                    catch (Exception ex)
                    {
                        // Log the exception, and ignore it. Continue execution.
                        Debug.WriteLine("Error loading seek index data.", ex);
                    }
                }

                // Hardware device priorities
                var deviceCandidates = new[]
                {
                    FFmpeg.AutoGen.AVHWDeviceType.AV_HWDEVICE_TYPE_CUDA,
                    FFmpeg.AutoGen.AVHWDeviceType.AV_HWDEVICE_TYPE_D3D11VA,
                    FFmpeg.AutoGen.AVHWDeviceType.AV_HWDEVICE_TYPE_DXVA2
                };

                // Hardware device selection
                if (videoStream.FPS <= 30)
                {
                    foreach (var deviceType in deviceCandidates)
                    {
                        var accelerator = videoStream.HardwareDevices.FirstOrDefault(d => d.DeviceType == deviceType);
                        if (accelerator == null) continue;
                        break;
                    }
                }
            }
        }


        /// <summary>
        /// Loads the index of the or create media seek.
        /// </summary>
        /// <param name="mediaFilePath">The URL.</param>
        /// <param name="streamIndex">The associated stream index.</param>
        /// <param name="durationSeconds">The duration in seconds.</param>
        /// <returns>
        /// The seek index
        /// </returns>
        private VideoSeekIndex LoadOrCreateVideoSeekIndex(string mediaFilePath, int streamIndex, double durationSeconds)
        {
            var seekFilePath = mediaFilePath + ".six";
            if (string.IsNullOrWhiteSpace(seekFilePath)) return null;

            if (File.Exists(seekFilePath))
            {
                using (var stream = File.OpenRead(seekFilePath))
                    return VideoSeekIndex.Load(stream);
            }
            else
            {
                if (durationSeconds <= 0)
                    return null;

                var seekIndex = Library.CreateVideoSeekIndex(mediaFilePath, streamIndex);
                if (seekIndex.Entries.Count <= 0) return null;

                using (var stream = File.OpenWrite(seekFilePath))
                    seekIndex.Save(stream);

                return seekIndex;
            }
        }

        private void B_SeekingStarted(object sender, RoutedEventArgs e)
        {
            Debug.WriteLine(e.OriginalSource);
        }
        

        private void Border_MouseWheel(object sender, MouseWheelEventArgs e)
        {
            ZoomLevelOffset = Math.Max(0, ZoomLevelOffset + e.Delta / 2000.0);
            CenterVideo();
        }

        private async void ButtonPlay_Click(object sender, RoutedEventArgs e)
        {
            MediaElement media;
            ToggleButton button = (sender as ToggleButton);
            if (button == null) return;
            if ((sender as ToggleButton).Name == "ButtonPlayB")
            {
                media = MediaB;
                if (MediaA.IsPlaying)
                    ButtonPlay_Click(MediaA, new RoutedEventArgs());
            }
            else
            {
                media = MediaA;
                if (MediaB.IsPlaying)
                    ButtonPlay_Click(MediaB, new RoutedEventArgs());
            }

            if (!(media.HasVideo || media.HasAudio)) return;
            if (media.IsPlaying)
            {
                await media.Pause();
            }
            else
            {
                if (media.HasMediaEnded)
                {
                    await media.Stop();
                }
                await media.Play();
                media.IsMuted = false;
            }
        }

        private void Border_MouseMove(object sender, MouseEventArgs e)
        {
            var border = sender as Border;
            MediaElement media = MediaA;
            if (border.Name == "BorderB")
            {
                media = MediaB;
            }
            var delta = Helpers.GetMousePos(this) - MouseDragStartPos;
            if (Mouse.MiddleButton == MouseButtonState.Pressed)
            {
                {
                    TranslateOffsetA = TranslateOffsetB = TranslateDragStartPos + delta * 2 / CurrentZoomLevel;
                    CenterVideo();
                    return;
                }
            }
            if (Mouse.LeftButton == MouseButtonState.Pressed)
            {
                // Hide cursor while mouse pressed
                Cursor = Cursors.None;
                if (delta.X < -10)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        media.Position = media.Position.Subtract(TimeSpan.FromSeconds(media.PositionStep.TotalSeconds * media.VideoFrameRate));
                    else
                        media.StepBackward();
                    SetCursorPos(MouseDragStartPos);
                    e.Handled = true;
                }
                else if (delta.X > 10)
                {
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                        media.Position = media.Position.Add(TimeSpan.FromSeconds(media.PositionStep.TotalSeconds * media.VideoFrameRate));
                    else
                        media.StepForward();
                    SetCursorPos(MouseDragStartPos);
                    e.Handled = true;
                }
            }
        }

        private void A_MediaInitializing(object sender, MediaInitializingEventArgs e)
        {
            if (InputA.MediaType == MediaType.ImageSequence)
            {
                e.Configuration.PrivateOptions["framerate"] = CurrentFrameRateEntry.CommandName;
                e.Configuration.PrivateOptions["start_number"] = InputA.First.ToString();
            }
            else if (InputA.MediaType == MediaType.Image)
            {
                e.Configuration.PrivateOptions["framerate"] = CurrentFrameRateEntry.CommandName;
                e.Configuration.PrivateOptions["loop"] = "1";
            }
            e.Configuration.GlobalOptions.SeekToAny = true;
        }

        private void B_MediaInitializing(object sender, MediaInitializingEventArgs e)
        {
            if (InputB.MediaType == MediaType.ImageSequence)
            {
                e.Configuration.PrivateOptions["framerate"] = CurrentFrameRateEntry.CommandName;
                e.Configuration.PrivateOptions["start_number"] = InputB.First.ToString();
            }
            else if (InputB.MediaType == MediaType.Image)
            {
                e.Configuration.PrivateOptions["framerate"] = CurrentFrameRateEntry.CommandName;
                e.Configuration.PrivateOptions["loop"] = "1";
            }
            e.Configuration.GlobalOptions.SeekToAny = true;
        }

        private void CenterVideo(bool adjustForZoom=true)
        {
            if (MediaA.NaturalVideoWidth > 0)
            {
                TransformsA.Scale.ScaleX = TransformsA.Scale.ScaleY = Math.Max(ZoomLevelTarget, ZoomLevelTarget + ZoomLevelOffset);
                TransformsA.Translate.X = BorderA.ActualWidth / 2.0 - (MediaA.NaturalVideoWidth / 2.0 * TransformsA.Scale.ScaleX) + (TranslateOffsetA.X / 2.0 * (adjustForZoom ? TransformsA.Scale.ScaleX : 1.0));
                TransformsA.Translate.Y = BorderA.ActualHeight / 2.0 - (MediaA.NaturalVideoHeight / 2.0 * TransformsA.Scale.ScaleX) + (TranslateOffsetA.Y / 2.0 * (adjustForZoom ? TransformsA.Scale.ScaleX : 1.0));
            }
            if (MediaB.NaturalVideoWidth > 0)
            {
                TransformsB.Scale.ScaleX = TransformsB.Scale.ScaleY = TransformsA.Scale.ScaleY;
                TransformsB.Translate.X = TransformsA.Translate.X;
                TransformsB.Translate.Y = TransformsA.Translate.Y;
            }
        }

        private string GetSizeString(MediaElement m, bool estimate=false)
        {
            string est = "";
            if (estimate)
            {
                double bpersec = MediaB.MediaStreamSize / GetNaturalDurationSeconds(MediaB);
                var estsize = GetNaturalDurationSeconds(MediaA) * bpersec;
                est = (estsize / 1024.0 / 1024.0) < 10.0 ? $" (Estimated Total: {(estsize / 1024.0).ToString("F2")} KB)" : $" (Estimated Total: {(estsize / 1024.0 / 1024.0).ToString("F2")} MB)";
            }
            return (m.MediaStreamSize / 1024.0 / 1024.0) < 10.0 ? $"{(m.MediaStreamSize / 1024.0).ToString("F2")} KB{est}" : $"{(m.MediaStreamSize / 1024.0 / 1024.0).ToString("F2")} MB{est}";
        }

        private string GetSizeString(FileMeta m, bool estimate = false)
        {
            string est = "";
            long size = m.Size;
            if (estimate)
            {
                double bpersec = size / GetNaturalDurationSeconds(MediaB);
                var estsize = GetNaturalDurationSeconds(MediaA) * bpersec;
                est = (estsize / 1024.0 / 1024.0) < 10.0 ? $" (Estimated Total: {(estsize / 1024.0).ToString("F2")} KB)" : $" (Estimated Total: {(estsize / 1024.0 / 1024.0).ToString("F2")} MB)";
            }
            return (size / 1024.0 / 1024.0) < 10.0 ? $"{(size / 1024.0).ToString("F2")} KB{est}" : $"{(size / 1024.0 / 1024.0).ToString("F2")} MB{est}";
        }

        private double GetNaturalDurationSeconds(MediaElement me, int fallbackSeconds=60)
        {
            if (me != null && me.NaturalDuration.HasValue)
            {
                return me.NaturalDuration.Value.TotalSeconds;
            }
            return fallbackSeconds;
        }
        private void MediaA_SourceUpdated(object sender, EventArgs e)
        {
            TimeSpan duration;
            if (MediaA.NaturalDuration == Duration.Forever || MediaA.NaturalDuration == Duration.Automatic || InputA.MediaType == MediaType.Image || !MediaA.NaturalDuration.HasValue)
            {
                duration = TimeSpan.FromSeconds(Properties.Settings.Default.defaultSingleImageDuration);
                double fps;
                if (double.TryParse(CurrentFrameRateEntry.Key, out fps))
                    MediaADurationAsFrames = TimeSpanToFrames(duration, fps);
                else
                    MediaADurationAsFrames = 0;
            }
            else {
                duration = MediaA.NaturalDuration.Value;
                MediaADurationAsFrames = TimeSpanToFrames(duration, MediaA.VideoFrameRate);
            }


            MediaA.Width = MediaA.NaturalVideoWidth;
            MediaA.Height = MediaA.NaturalVideoHeight;

            if (CurrentSettings == null)
            {
                CurrentSettings = new RenderSettingsItem();
                CurrentSettings.RangeStart = TimeSpan.FromSeconds(0);
                CurrentSettings.RangeEnd = duration;
            }
            RangeSliderPlayA_RangeLowerValue = CurrentSettings.RangeStart.TotalSeconds;
            RangeSliderPlayA_RangeUpperValue = CurrentSettings.RangeEnd.TotalSeconds;

            string fileInfo = "";
            if (MediaA.MediaInfo != null)
            {
                foreach (var item in MediaA.MediaInfo.BestStreams)
                {
                    var i = item.Value;
                    if (i.CodecType == FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_AUDIO)
                    {
                        fileInfo += $"Audio: {i.CodecName} Bitrate: {i.BitRate} ";
                    }
                    else if (i.CodecType == FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_VIDEO)
                    {
                        fileInfo += $"Video: {i.CodecName} FPS: {i.FPS:F2} Bitrate: {i.BitRate} ";
                    }
                }
            }

            if (InputA.MediaType == MediaType.ImageSequence)
            {
                LabelLeftStatus_ContentValue = $"{GetSizeString(InputA)} ({fileInfo})";
            }
            else
            {
                LabelLeftStatus_ContentValue = $"{GetSizeString(MediaA)} ({fileInfo})";
            }

            MediaA.Width = MediaA.NaturalVideoWidth;
            MediaA.Height = MediaA.NaturalVideoHeight;
            if (_resetZoom) { SetZoomLevel(-1.0, resetTranslate: true); _resetZoom = false; }
            CenterVideo();
            
            // Enable Buttons
            ButtonSendToQueue.IsEnabled = true;
            ButtonRenderFinal.IsEnabled = true;
            ButtonRenderPreview.IsEnabled = true;
        }

        private void MediaB_SourceUpdated(object sender, EventArgs e)
        {
            if (MediaB.NaturalDuration.HasValue)
                MediaBDurationAsFrames = TimeSpanToFrames(MediaB.NaturalDuration.Value, MediaB.VideoFrameRate);
            else
                MediaBDurationAsFrames = 0;
            MediaB.Width = MediaB.NaturalVideoWidth;
            MediaB.Height = MediaB.NaturalVideoHeight;
            var offset = TimeSpan.FromSeconds(0);
            var ending = MediaB.NaturalDuration.Value;

            string fileInfo = "";
            foreach (var item in MediaB.MediaInfo.BestStreams)
            {
                var i = item.Value;
                if (i.CodecType == FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_AUDIO)
                {
                    fileInfo += $"Audio: {i.CodecName} Bitrate: {i.BitRate} ";
                }
                else if (i.CodecType == FFmpeg.AutoGen.AVMediaType.AVMEDIA_TYPE_VIDEO)
                {
                    fileInfo += $"Video: {i.CodecName} FPS: {i.FPS:F2} Bitrate: {i.BitRate} ";
                }
                //foreach (PropertyInfo k in item.Value.GetType().GetProperties())
                //{
                //    Debug.WriteLine(k.ToString());
                //}
            }

            if (CurrentSettings != null)
            {
                if (ToggleManualRangeMode_Value == true)
                {
                    offset = CurrentSettings.RangeStart;
                    ending = CurrentSettings.RangeEnd.Duration();
                }
                else
                {
                    offset = CurrentSettings.RangeStart;
                    ending = CurrentSettings.RangeEnd;
                }
                MediaB.Tag = offset;

                if (InputB.MediaType == MediaType.ImageSequence)
                {
                    LabelRightStatus_ContentValue = $"{GetSizeString(InputB, estimate: true)} {CurrentSettings.EncoderVideo.Name} ({CurrentSettings.EncoderVideo.QualityMode} {CurrentSettings.QualityVideo}) ({fileInfo})";
                }
                else
                {
                    LabelRightStatus_ContentValue = $"{GetSizeString(MediaB, estimate:true)} {CurrentSettings.EncoderVideo.Name} ({CurrentSettings.EncoderVideo.QualityMode} {CurrentSettings.QualityVideo}) ({fileInfo})";
                }
            }
            LabelMediaBLeft_ContentValue = MediaB.Position.ToString(@"d\.hh\:mm\:ss\.ff") + $" ({TimeSpanToFrames(MediaB.Position, MediaB.VideoFrameRate)})";
            LabelMediaBInPoint_ContentValue = offset.ToString(@"d\.hh\:mm\:ss\.ff") + $" ({TimeSpanToFrames(offset, MediaB.VideoFrameRate)})";
            LabelMediaBOutPoint_ContentValue = ending.ToString(@"d\.hh\:mm\:ss\.ff") + $" ({TimeSpanToFrames(ending, MediaB.VideoFrameRate)})";

            if (_resetZoom) { SetZoomLevel(-1.0, resetTranslate: true); _resetZoom = false; }
            CenterVideo();
        }


        private async void MediaA_PositionChanged(object o, EventArgs e)
        {
            MediaAPositionAsFrames = TimeSpanToFrames(MediaA.Position, MediaA.VideoFrameRate);
            if (DoPlayRanged)
            {
                if (MediaA.Position.TotalSeconds < RangeSliderPlayA_RangeLowerValue)
                {
                    MediaA.Position = TimeSpan.FromSeconds(RangeSliderPlayA_RangeLowerValue);
                }
                if (MediaA.Position.TotalSeconds > RangeSliderPlayA_RangeUpperValue)
                {
                    await MediaA.Pause();
                    await MediaA.Seek(TimeSpan.FromSeconds(RangeSliderPlayA_RangeLowerValue));
                    if (ToggleLoopedPlayback_Value)
                    {
                        await MediaA.Play();
                    }
                    else
                    {
                        DoPlayRanged = false;
                    }
                }
            }
        }

        public int TimeSpanToFrames(TimeSpan? ts, double fps=25)
        {
            if (ts == null) return 0;
            return (int)Math.Round(((TimeSpan)ts).TotalSeconds * fps);
        }

        private void MediaB_PositionChanged(object o, EventArgs e)
        {
            MediaBPositionAsFrames = TimeSpanToFrames(MediaB.Position, MediaB.VideoFrameRate);
            try
            {
                var offset = TimeSpan.FromSeconds(0);
                if (CurrentSettings != null)
                {
                    offset = CurrentSettings.RangeStart;
                }
                var adjustedPosition = MediaB.Position + offset;
                if (ToggleLinkedPlayers_Value)
                {
                    MediaA.Position = adjustedPosition;
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error during PositionChanged(B): {ex.Message}");
            }
        }


        private void SeekToSlider(MediaElement me, Slider slider, bool isPreviewMediaElement=false)
        {
            if (me.Source == null) return;
            double offset = 0;
            if (isPreviewMediaElement)
            {
                if (CurrentSettings != null)
                {
                    offset = CurrentSettings.RangeStart.TotalSeconds;
                }
            }
            var adjustedValue = slider.Value - offset;
            double seconds = Helpers.Clamp(adjustedValue, 0, GetNaturalDurationSeconds(me));
            me.Position = TimeSpan.FromSeconds(seconds);
        }


        private void ButtonRenderPreview_Click(object sender, RoutedEventArgs e)
        {
            //RenderPreview(NumericSeconds.Value);
            Render(isPreview:true);
        }

        public void DirectToQueue(string fileName)
        {
            var input = OpenFile(fileName,infoOnly:true);
            input.mi = new MediaInfoDotNet.MediaFile(fileName);
            var fn = GetSaveFileName(rs:null, input:input, generateAutoFileName:true);
            fn = Path.Combine(Properties.Settings.Default.defaultOutputPath, fn);
            if (input != null) Render(addHistory: false, useRange: false, outputPath:fn, sendToQueue: true, inputFileMeta: input);
        }

        /// <summary>
        /// Starts the render process
        /// </summary>
        /// <param name="addHistory">Whether to add an entry to the render history or not</param>
        /// <param name="useRange">Whether to render only the part of the input defined by range, or the whole video</param>
        /// <param name="outputPath">Set to override the output path, otherwise the video is rendered to the temp folder</param>
        private void Render(bool addHistory = true, bool useRange = true, string outputPath = null, bool isPreview = false, bool saveAsPresetOnly = false, bool sendToQueue = false, FileMeta inputFileMeta=null)
        {
            if (inputFileMeta == null && (!MediaA.IsOpen || (!MediaA.HasVideo && !MediaA.HasAudio))) return;
            FileMeta fileMeta;
            if (inputFileMeta==null)
            {
                fileMeta = InputA;
            }
            else
            {
                fileMeta = inputFileMeta;
            }
            
            TimeSpan startPreview, endPreview, renderDuration;

            if (!sendToQueue)
            {
                ButtonRenderPreview.IsEnabled = false;
                ButtonRenderFinal.IsEnabled = false;
            }

            TempSettings = CurrentSettings.GetClone();
            TempSettings.MediaType = fileMeta.MediaType;
            TempSettings.AudioSampleRate = (AudioSampleRate)ComboAudioSampleRate.SelectedIndex;

            if (!MediaA.HasVideo && MediaA.HasAudio)
            {
                TempSettings.EncoderVideo = VideoCodec.Settings(VideoCodec.None);
                if (TempSettings.EncoderAudio.FileExtension == null)
                {
                    TempSettings.EncoderAudio.FileExtension = fileMeta.ExtensionLower;
                }
                if (outputPath == null)
                {
                    outputPath = Path.Combine(Path.GetTempPath(), TempSettings.guid.ToString() + TempSettings.EncoderAudio.FileExtension);
                }
            }
            else
            {
                if (TempSettings.EncoderVideo.FileExtension == null)
                {
                    TempSettings.EncoderVideo.FileExtension = fileMeta.ExtensionLower;
                }
                if(outputPath == null)
                {
                    outputPath = Path.Combine(Path.GetTempPath(), TempSettings.guid.ToString() + TempSettings.EncoderVideo.FileExtension);
                }
            }

            if (useRange == true)
            {
                if (ToggleManualRangeMode_Value == false)
                {
                    RangeSliderPlayA_RangeLowerValue = Math.Min(GetNaturalDurationSeconds(MediaA) - +(double)Properties.Settings.Default.defaultPreviewDuration, MediaA.Position.TotalSeconds);
                    RangeSliderPlayA_RangeUpperValue = Math.Min(MediaA.Position.TotalSeconds + (double)Properties.Settings.Default.defaultPreviewDuration, GetNaturalDurationSeconds(MediaA));
                }
                startPreview = TimeSpan.FromSeconds(RangeSliderPlayA_RangeLowerValue);
                endPreview = TimeSpan.FromSeconds(RangeSliderPlayA_RangeUpperValue);
                renderDuration = endPreview - startPreview;
            }
            else if (inputFileMeta != null)
            {
                startPreview = TimeSpan.FromSeconds(0);
                endPreview = TimeSpan.FromMilliseconds(inputFileMeta.mi.General.Duration);
                renderDuration = endPreview;
                
            }
            else if (MediaA != null)
            {
                startPreview = TimeSpan.FromSeconds(0);
                endPreview = MediaA.NaturalDuration.Value;
                renderDuration = MediaA.NaturalDuration.Value;
            }
            else
            {
                return;
            }

            TempSettings.Options = new ConversionOptions
            {
                VideoEncoder = TempSettings.EncoderVideo,
                AudioEncoder = TempSettings.EncoderAudio,
                QualityVideo = TempSettings.QualityVideo,
                QualityAudio = TempSettings.QualityAudio,
                AudioSampleRate = TempSettings.AudioSampleRate,
                Seek = startPreview,
                MaxVideoDuration = renderDuration,
            };

            if (fileMeta.MediaType == MediaType.ImageSequence || fileMeta.MediaType == MediaType.Image)
            {
                if (fileMeta.MediaType == MediaType.Image)
                {
                    TempSettings.InputFile = new MediaFile(fileMeta.Info.FullName);
                    TempSettings.Options.PreArgs = "-loop 1";
                }
                else
                {
                    TempSettings.InputFile = new MediaFile(fileMeta.GetSequenceString());
                    TempSettings.Options.PreArgs = $"-start_number {fileMeta.First}";
                }
                TempSettings.Options.VideoFps = TempSettings.Options.InputFps = CurrentFrameRateEntry.CommandName;
                TempSettings.FrameRate = CurrentFrameRateEntry;
            }
            else
            {
                TempSettings.InputFile = new MediaFile(fileMeta.Info.FullName);
                TempSettings.FrameRate = new FrameRates.FrameRateEntry(MediaA.VideoFrameRate.ToString(), MediaA.VideoFrameRate.ToString());
            }

            TempSettings.OutputFile = new MediaFile(Path.Combine(outputPath));

            if (ToggleReplaceAudio_Value == true)
            {
                if (File.Exists(TextBoxAudioFileName.Text))
                {
                    TempSettings.ReplaceAudio = true;
                    TempSettings.ReplaceAudioFileName = TextBoxAudioFileName.Text;
                    TempSettings.Options.SecondInput = new MediaFile(TextBoxAudioFileName.Text);
                    TempSettings.Options.Mappings.Add(new ChannelMapping(0, 0));
                    TempSettings.Options.Mappings.Add(new ChannelMapping(1, 0));
                }
            }

            //
            // VIDEO FILTERS
            //

            // SCALE FILTER
            List<string> scaleFilters = new List<string>();

            TempSettings.ResizeVideo = ToggleResizeVideo_Value;
            if (ToggleResizeVideo_Value == true)
            {
                TempSettings.Options.CustomWidth = TempSettings.Width;
                TempSettings.Options.CustomHeight = TempSettings.Height;
                scaleFilters.Add($"{TempSettings.Options.CustomWidth ?? -2}:{TempSettings.Options.CustomHeight ?? -2}");
            }

            var color_range_in = TempSettings.Color_Range_In = (ComboBoxColorRangeIn.SelectedItem as ComboBoxItem).Tag.ToString();
            var color_range_out = TempSettings.Color_Range_Out = (ComboBoxColorRangeOut.SelectedItem as ComboBoxItem).Tag.ToString();
            if (color_range_in != "default" || color_range_out != "default")
            {
                if (color_range_in == "default") color_range_in = "pc";
                if (color_range_out == "default") color_range_out = "pc";
                scaleFilters.Add($"in_range={color_range_in}:out_range={color_range_out}");
            }

            if (scaleFilters.Count > 0)
            {
                TempSettings.Options.VideoFilters.Add($"scale={string.Join(":", scaleFilters.ToArray())}");
            }

            // LUT 

            TempSettings.ApplyLUT = ToggleCheckApplyLUT_Value;
            if (ToggleCheckApplyLUT_Value == true)
            {
                if (File.Exists(TextBoxLUTFileName.Text))
                {
                    FileInfo lut = new FileInfo(TextBoxLUTFileName.Text);
                    TempSettings.Options.VideoFilters.Add($"lut3d=\\'{lut.FullName.Replace('\\','/')}\\'");
                    TempSettings.LUT = TextBoxLUTFileName.Text;
                }
            }

            // EQ

            TempSettings.ApplyGamma = ToggleCheckApplyGamma_Value;
            TempSettings.Gamma = TextBoxGamma.Text;
            if (ToggleCheckApplyGamma_Value == true)
            {
                TempSettings.Options.VideoFilters.Add($"eq=gamma={TextBoxGamma.Text}");
            }

            // EXTRA

            TempSettings.ExtraVideoFilters = TextBoxExtraArgsVideoFilter.Text;
            if (TextBoxExtraArgsVideoFilter.Text != "")
            {
                TempSettings.Options.VideoFilters.Add($"{TextBoxExtraArgsVideoFilter.Text}");
            }

            // TIMECODE

            TempSettings.SetTimeCode = ToggleCheckSetTimeCode_Value;
            TempSettings.BurnTimeCode = ToggleCheckBurnInTimeCode_Value;

            if (ToggleCheckSetTimeCode_Value == true)
            {
                TempSettings.TCh = TextBoxTCh.Text;
                TempSettings.TCm = TextBoxTCm.Text;
                TempSettings.TCs = TextBoxTCs.Text;
                TempSettings.TCf = TextBoxTCf.Text;
            }
            else
            {
                TempSettings.TCh = startPreview.Hours.ToString();
                TempSettings.TCm = startPreview.Minutes.ToString();
                TempSettings.TCs = startPreview.Seconds.ToString();
                TempSettings.TCf = (TimeSpanToFrames(startPreview, MediaA.VideoFrameRate) % MediaA.VideoFrameRate).ToString();
            }

            string tc = $"{TempSettings.TCh}:{TempSettings.TCm}:{TempSettings.TCs}:{TempSettings.TCf}";
            TempSettings.Options.TimeCode = tc;

            if (ToggleCheckBurnInTimeCode_Value == true)
            {
                var fs = Properties.Settings.Default.TimeCodeFontSize;
                var pos = $"x = ({TempSettings.Width} - tw) / 2: y = {TempSettings.Height} - (2 *{ fs})";
                string tc_escaped = $"{TempSettings.TCh}\\:{TempSettings.TCm}\\:{TempSettings.TCs}\\:{TempSettings.TCf}";
                var r = $"{ MediaA.VideoFrameRate.ToString(new NumberFormatInfo() { NumberDecimalSeparator = "." }) }";
                TempSettings.Options.VideoFilters.Add($"drawtext=fontfile='{DrawFont.Name}': timecode='{tc_escaped}': r={r}: {pos}: fontsize={fs}: fontcolor={Properties.Settings.Default.TimeCodeFontColor}: box=1: boxcolor=0x00000099");
            }

            // VIDEO ENCODER

            if (TempSettings.Options.VideoEncoder.MinBitRate != null)
            {
                if (TempSettings.Options.VideoEncoder.MinBitRate == 0)
                {
                    TempSettings.Options.VideoMinBitRate = TempSettings.QualityVideo;
                }
                else if (TempSettings.Options.VideoEncoder.MinBitRate > 0)
                {
                    TempSettings.Options.VideoMinBitRate = TempSettings.Options.VideoEncoder.MinBitRate;
                }
            }
            if (TempSettings.Options.VideoEncoder.MaxBitRate != null)
            {
                if (TempSettings.Options.VideoEncoder.MaxBitRate == 0)
                {
                    TempSettings.Options.VideoMaxBitRate = TempSettings.QualityVideo;
                }
                else if (TempSettings.Options.VideoEncoder.MaxBitRate > 0)
                {
                    TempSettings.Options.VideoMaxBitRate = TempSettings.Options.VideoEncoder.MaxBitRate;
                }
            }
            if (TempSettings.Options.VideoEncoder.BufferBitRate != null)
            {
                if (TempSettings.Options.VideoEncoder.BufferBitRate == 0)
                {
                    TempSettings.Options.VideoBufferBitRate = TempSettings.QualityVideo;
                }
                else if (TempSettings.Options.VideoEncoder.BufferBitRate > 0)
                {
                    TempSettings.Options.VideoBufferBitRate = TempSettings.Options.VideoEncoder.BufferBitRate;
                }
                else if (TempSettings.Options.VideoEncoder.BufferBitRate == -1)
                {
                    double fps;
                    //if (double.TryParse(FrameRates.FrameRateCollection[Properties.Settings.Default.defaultFrameRate].Key, out fps))
                    if (double.TryParse(CurrentFrameRateEntry.Key, out fps))
                    {
                        TempSettings.Options.VideoBufferBitRate = (int)(TempSettings.QualityVideo / (TempSettings.Options.VideoFps == null ? MediaA.VideoFrameRate : fps));
                    }
                    else
                    {
                        TempSettings.Options.VideoBufferBitRate = (int)(TempSettings.QualityVideo / MediaA.VideoFrameRate);
                    }
                }
            }

            if (ComboPixFmt.SelectedValue != null)
                TempSettings.Options.PixFmt = TempSettings.PixFmt = ComboPixFmt.SelectedValue.ToString();

            
            TempSettings.SourcePath = fileMeta.Info.FullName;
            TempSettings.OutputPath = outputPath;
            TempSettings.RangeStart = startPreview;
            TempSettings.RangeEnd = endPreview;
            TempSettings.Duration = renderDuration;
            TempSettings.IsPreview = isPreview;

            RangeSliderPlayA_RangeLowerValue = startPreview.TotalSeconds;
            RangeSliderPlayA_RangeUpperValue = endPreview.TotalSeconds;

            TempSettings.FfmpegParameters = ffmpeg.GetConversionString(TempSettings.InputFile, TempSettings.OutputFile, TempSettings.Options);

            if (saveAsPresetOnly)
            {
                RenderSettingsItem.ToJsonFileWithDialog(TempSettings);
                ReadPresetsMenuItems(Menu_PresetsRoot);
            }
            else if (sendToQueue)
            {
                TempSettings.Status = RenderQueueItemStatus.Queued;
                TempSettings.Queued = true;
                RenderQueue.Add(TempSettings);
                ShowRenderQueue();
            }
            else
            {
                TempSettings.Status = RenderQueueItemStatus.Preview;
                TempSettings.Queued = false;
                ShowRightPlayer();
                RenderFromRenderSettings(TempSettings);
            }
        }

        public void RenderAllQueued()
        {
            QueueIsRendering = true;
            SetQueuedWaiting();
            StartNextWaiting();
            if (GetQueuedStatusCount(RenderQueueItemStatus.Waiting) == 0 && GetQueuedStatusCount(RenderQueueItemStatus.Encoding) == 0)
            {
                QueueIsRendering = false;
            }
        }

        public void SetQueuedWaiting()
        {
            foreach (var item in RenderQueue)
            {
                if (item.Status == RenderQueueItemStatus.Queued)
                {
                    item.Status = RenderQueueItemStatus.Waiting;
                }
            }
        }

        public int GetQueuedStatusCount(RenderQueueItemStatus status=RenderQueueItemStatus.Encoding)
        {
            return RenderQueue.Where((x) => x.Status == status).Count();
        }

        public void StartNextWaiting()
        {
            foreach (var item in RenderQueue.Where((x)=>x.Status==RenderQueueItemStatus.Waiting))
            {
                if (GetQueuedStatusCount(RenderQueueItemStatus.Encoding) < MaxConcurrentEncoding)
                {
                    RenderFromRenderSettings(item);
                }
                else
                {
                    break;
                }
            }
        }

        public async void RenderFromRenderSettings(RenderSettingsItem rs)
        {
            rs.cancellationTokenSource = new CancellationTokenSource();
            rs.Progress = 0.0;
            rs.Options.renderSettingsItem = rs;

            if (rs.Queued==false)
            {
                ProgressFF.Value = 0;
                ButtonStopRender.IsEnabled = true;
                RenderCancel = rs.cancellationTokenSource;
            }

            try
            {
                rs.Status = RenderQueueItemStatus.Encoding;
                rs.Task = await ffmpeg.ConvertAsync(rs.InputFile, rs.OutputFile, rs.Options, cancellationToken: rs.cancellationTokenSource.Token);
                if (rs.Task.FileInfo.Exists && rs.Status==RenderQueueItemStatus.Finished)
                {
                    if (rs.Queued == false)
                    {
                        CurrentSettings = TempSettings.DeepClone();
                        try
                        {
                            RenderHistory.Add(TempSettings);
                        }
                        catch (System.ArgumentException)
                        {
                            RenderHistory.Add(TempSettings.DeepClone().NewGuid());
                        }
                        if (WinRenderHistory != null && WinRenderHistory.IsLoaded == true)
                            WinRenderHistory.RefreshItems();
                    }
                }
                else
                {
                    if (rs.Status == RenderQueueItemStatus.Error)
                    {
                        new WindowErrorWithLog()
                        {
                            Title = "Error",
                            ErrorMessage = "Error during render job",
                            ErrorLog = TempSettings.Log
                        }
                        .ShowDialog();
                    }
                }
            }
            catch (System.Threading.Tasks.TaskCanceledException)
            {
                rs.Status = RenderQueueItemStatus.Stopped;
                if (File.Exists(rs.OutputPath))
                {
                    try
                    {
                        File.Delete(rs.OutputPath);
                    }
                    catch (Exception)
                    {
                        Debug.WriteLine($"Could not delete {rs.OutputPath}");
                    }
                }
            }
            catch (NullReferenceException ex)
            {
                // for some reason VP9 encoding causes an exception
                Debug.WriteLine(ex.Message);
            }
            catch (Exception ex)
            {
                new WindowErrorWithLog()
                {
                    Title = "Error",
                    ErrorMessage = "Error during render job",
                    ErrorLog = ex.Message
                }
                .ShowDialog();
            }
            WriteToLog("-----\nffmpeg command parameters were:\n" + TempSettings.FfmpegParameters + "\n-----\n");
            if (rs.Queued == false)
            {
                ProgressFF.Value = 100;
                ButtonStopRender.IsEnabled = false;
                ButtonRenderPreview.IsEnabled = true;
                ButtonRenderFinal.IsEnabled = true;
            }
        }


        private void SaveCurrentSettingsAsPreset()
        {
            TempSettings = CurrentSettings.GetClone();
            TempSettings.AudioSampleRate = (AudioSampleRate)ComboAudioSampleRate.SelectedIndex;

            if (InputA != null)
            {
                if (InputA.MediaType == MediaType.ImageSequence || InputA.MediaType == MediaType.Image)
                {
                    TempSettings.FrameRate = CurrentFrameRateEntry;
                }
                else
                {
                    TempSettings.FrameRate = new FrameRates.FrameRateEntry(MediaA.VideoFrameRate.ToString(), MediaA.VideoFrameRate.ToString());
                }
            }
            else
            {
                TempSettings.FrameRate = new FrameRates.FrameRateEntry("25","25");
            }

            TempSettings.ReplaceAudio = ToggleReplaceAudio_Value;
            TempSettings.ReplaceAudioFileName = TextBoxAudioFileName.Text;
            TempSettings.ResizeVideo = ToggleResizeVideo_Value;
            TempSettings.ApplyLUT = ToggleCheckApplyLUT_Value;
            TempSettings.LUT = TextBoxLUTFileName.Text;
            TempSettings.SetTimeCode = ToggleCheckSetTimeCode_Value;
            TempSettings.BurnTimeCode = ToggleCheckBurnInTimeCode_Value;
            TempSettings.TCh = TextBoxTCh.Text;
            TempSettings.TCm = TextBoxTCm.Text;
            TempSettings.TCs = TextBoxTCs.Text;
            TempSettings.TCf = TextBoxTCf.Text;
            TempSettings.PixFmt = ComboPixFmt.SelectedValue.ToString();
            TempSettings.ApplyGamma = ToggleCheckApplyGamma_Value;
            TempSettings.Gamma = TextBoxGamma.Text;
            TempSettings.Color_Range_In = (ComboBoxColorRangeIn.SelectedItem as ComboBoxItem).Tag.ToString();
            TempSettings.Color_Range_Out = (ComboBoxColorRangeOut.SelectedItem as ComboBoxItem).Tag.ToString();
            TempSettings.ExtraVideoFilters = TextBoxExtraArgsVideoFilter.Text;

            RenderSettingsItem.ToJsonFileWithDialog(TempSettings);
            ReadPresetsMenuItems(Menu_PresetsRoot);
        }

        private void RangeSliderPlayA_LowerValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            var ts = TimeSpan.FromSeconds(e.NewValue);
            LabelMediaAInPoint.Content = ts.ToString(@"d\.hh\:mm\:ss\.ff") + $" ({TimeSpanToFrames(ts, MediaA.VideoFrameRate)})";
        }

        private void RangeSliderPlayA_UpperValueChanged(object sender, RangeParameterChangedEventArgs e)
        {
            var ts = TimeSpan.FromSeconds(e.NewValue);
            LabelMediaAOutPoint.Content = ts.ToString(@"d\.hh\:mm\:ss\.ff") + $" ({TimeSpanToFrames(ts, MediaA.VideoFrameRate)})";
        }

        private void ButtonShowPreferences_Click(object sender, RoutedEventArgs e)
        {
            if (WinPreferences == null || WinPreferences.IsLoaded == false)
            {
                WinPreferences = new WindowPreferences(this);
            }
            WinPreferences.Show();
        }

        private void ComboEncoderVideo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboEncoderVideo.SelectedItem == null) return;
            if (CurrentSettings == null) CurrentSettings = new RenderSettingsItem();
            CurrentSettings.EncoderVideo = ComboEncoderVideo.SelectedItem as VideoCodecEntry;
            SliderQuality_MinValue = CurrentSettings.EncoderVideo.QualityMin;
            SliderQuality_MaxValue = CurrentSettings.EncoderVideo.QualityMax;
            SliderQuality_StepValue = CurrentSettings.EncoderVideo.QualityStep;
            CurrentSettings.QualityVideo = CurrentSettings.EncoderVideo.QualityDefault;
            
            if (CurrentSettings.EncoderVideo.QualityMode == null)
            {
                if (SliderQualityVideo != null)
                {
                    SliderQualityVideo.IsEnabled = false;
                }
                LabelQuality_Content = "n/a";
                SliderQuality_QualityValue = 0;
            }
            else
            {
                if (SliderQualityVideo != null)
                {
                    SliderQualityVideo.IsEnabled = true;
                }
                SliderQuality_QualityValue = CurrentSettings.QualityVideo;
                LabelQuality_Content = CurrentSettings.EncoderVideo.QualityMode;
            }

            if (ToggleAutoPreview_Value == true)
            {
                Render(isPreview:true);
            }
            if (ComboPixFmt!=null) UpdatePixFmts();
        }

        private void UpdatePixFmts()
        {
            SupportedPixFmts = ffmpeg.GetSupportedPixFmts(CurrentSettings.EncoderVideo.Encoder);
            if (SupportedPixFmts == null) return;
            if (CurrentSettings.PixFmt != "" && SupportedPixFmts.Contains(CurrentSettings.PixFmt))
            {
                ComboPixFmt.SelectedValue = CurrentSettings.PixFmt;
            }
            else if (CurrentSettings.EncoderVideo.PixFmt != "" && SupportedPixFmts.Contains(CurrentSettings.EncoderVideo.PixFmt))
            {
                ComboPixFmt.SelectedValue = CurrentSettings.EncoderVideo.PixFmt;
            }
            else
            {
                ComboPixFmt.SelectedIndex = 0;
            }
        }

        private void BorderA_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void BorderB_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                e.Effects = DragDropEffects.Move;
            }
            else
            {
                e.Effects = DragDropEffects.None;
            }
        }

        private void BorderA_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    OpenFile(files[0]);
                }
            }
            e.Effects = DragDropEffects.None;
        }

        private void BorderB_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                if (files.Length > 0)
                {
                    OpenFile(files[0]);
                }
            }
            e.Effects = DragDropEffects.None;
        }

        private void OpenFile()
        {
            string dir = Properties.Settings.Default.lastOpenedDirectory;
            if (!Directory.Exists(dir))
                dir = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);
            WinSequenceDialog = new SequenceDialog(this, dir);
            WinSequenceDialog.Show();
        }

        private void OpenFileStandard()
        {
            string dir = Properties.Settings.Default.lastOpenedDirectory;
            OpenFileDialog openFileDialog = new OpenFileDialog()
            {
                InitialDirectory = Properties.Settings.Default.lastOpenedDirectory
            };
            if (openFileDialog.ShowDialog() == true)
            {
                MouseDragStartPos = Helpers.GetMousePos(this);
                OpenFile(openFileDialog.FileName);
            }
        }

        public FileMeta OpenFile(string path, bool infoOnly=false)
        {
            var input = FileMeta.Create(path);

            if (input == null || input.MediaType == MediaType.Unknown || input.MediaType == MediaType.Directory)
            {
                MessageBox.Show($"Cannot open file {path}. Error reading file or file type {Path.GetExtension(path)} not allowed.", "Cannot open file", MessageBoxButton.OK);
                return null;
            }
            else if (input.MediaType == MediaType.ImageSequence || input.MediaType == MediaType.Image)
            {
                ComboInputFpsLabel.Visibility = ComboInputFps.Visibility = Visibility.Visible;
                ComboInputFps.IsEnabled = true;
            }
            else if (input.MediaType == MediaType.Audio)
            {
                ComboInputFpsLabel.Visibility = ComboInputFps.Visibility = Visibility.Hidden;
                ComboInputFps.IsEnabled = false;
            }
            else
            {
                ComboInputFpsLabel.Visibility = ComboInputFps.Visibility = Visibility.Hidden;
                ComboInputFps.IsEnabled = false;
            }

            if (input.mi == null) input.mi = new MediaInfoDotNet.MediaFile(path);

            if (!infoOnly)
            {
                _resetZoom = true;
                InputA = input;
                InputB = null;
                //UpdateMediaSources(onlyLeftSide: true, forceReload: true);
                UpdateMediaSources(forceReload: true);
                SliderPlayA.Value = 0;

                AddRecentFile(path);
            }
            return input;
        }

        private void AddRecentFile(string path)
        {
            if (Properties.Settings.Default.RecentFiles.Contains(path)) return;
            var count = Properties.Settings.Default.RecentFiles.Count;
            Properties.Settings.Default.RecentFiles.Insert(0, path);
            if (Properties.Settings.Default.RecentFiles.Count > 5)
            {
                try
                {
                    Properties.Settings.Default.RecentFiles.RemoveAt(count);
                }
                catch
                {
                }
            }
            Properties.Settings.Default.Save();
            Menu_Open_Recent.Items.Refresh();
        }

        public void OpenFile(FileMeta fm)
        {
            if (fm == null || fm.MediaType == MediaType.Unknown || fm.MediaType == MediaType.Directory)
            {
                MessageBox.Show($"Cannot open file {fm.DisplayName}.", "Cannot open file", MessageBoxButton.OK);
            }
            else
            {
                _resetZoom = true;
                InputA = fm;
                UpdateMediaSources(onlyLeftSide: true);
            }
        }

        private void SliderQualityVideo_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentSettings.QualityVideo = (int)Math.Round(e.NewValue);
        }

        private void SliderQualityVideo_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ToggleAutoPreview_Value == true)
            {
                Render(isPreview:true);
            }
        }

        private void SliderQualityAudio_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            CurrentSettings.QualityAudio = (int)Math.Round(e.NewValue);
        }

        private void SliderQualityAudio_DragCompleted(object sender, DragCompletedEventArgs e)
        {
            if (ToggleAutoPreview_Value == true)
            {
                Render(isPreview:true);
            }
        }

        private void ButtonStopRender_Click(object sender, RoutedEventArgs e)
        {
            if (RenderCancel.Token.CanBeCanceled)
            {
                RenderCancel.CancelAfter(1);
            }
        }

        
        private void Menu_Quit_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Application.Current.Shutdown();
        }

        private void Menu_Open_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFile();
        }

        private void ButtonToggleDualScreen_Click(object sender, RoutedEventArgs e)
        {
            if (RightColumn.Width.Value < 1)
            {
                ShowRightPlayer();
            }
            else
            {
                HideRightPlayer();
            }
        }

        private void ShowRightPlayer()
        {
            if (RightColumn.Width.Value > 1)
                return;
            RightColumn.Width = new GridLength(1, GridUnitType.Star);
            GridSplitView.InvalidateVisual();
            GridSplitView.UpdateLayout();
            CenterVideo();
        }

        private void HideRightPlayer()
        {
            if (RightColumn.Width.Value < 1)
                return;
            RightColumn.Width = new GridLength(0);
            GridSplitView.InvalidateVisual();
            GridSplitView.UpdateLayout();
            CenterVideo();
        }

        private void FFMainWindow_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            CenterVideo();
        }

        private void ComboEncoderAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboEncoderAudio.SelectedItem == null) return;
            if (CurrentSettings == null) CurrentSettings = new RenderSettingsItem();
            CurrentSettings.EncoderAudio = ComboEncoderAudio.SelectedItem as AudioCodecEntry;
            SliderQualityAudio_MinValue = CurrentSettings.EncoderAudio.QualityMin;
            SliderQualityAudio_MaxValue = CurrentSettings.EncoderAudio.QualityMax;
            SliderQualityAudio_StepValue = CurrentSettings.EncoderAudio.QualityStep;
            CurrentSettings.QualityAudio = CurrentSettings.EncoderAudio.QualityDefault;

            if (CurrentSettings.EncoderAudio.QualityMode == null)
            {
                if (SliderQualityAudio != null)
                {
                    SliderQualityAudio.IsEnabled = false;
                }
                LabelQualityAudio_Content = "n/a";
                SliderQualityAudio_QualityValue = 0;
            }
            else
            {
                if (SliderQualityAudio != null)
                {
                    SliderQualityAudio.IsEnabled = true;
                }
                SliderQualityAudio_QualityValue = CurrentSettings.QualityAudio;
                LabelQualityAudio_Content = CurrentSettings.EncoderAudio.QualityMode;
            }

            if (ToggleAutoPreview_Value == true)
            {
                Render(isPreview:true);
            }
        }

        private void ButtonAudioFileName_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                TextBoxAudioFileName.Text = openFileDialog.FileName;
            }
        }


        private void ToggleOverlayHelp(object sender, RoutedEventArgs e)
        {
            ToggleOverlayHelp();
        }

        public void ToggleOverlayHelp()
        {
            OverlayInfoA.Text = Helpers.HelpText;
            if (OverlayFadeA.Visibility == Visibility.Hidden)
            {
                OverlayFadeA.Visibility = Visibility.Visible;
            }
            else
            {
                OverlayFadeA.Visibility = Visibility.Hidden;
            }
        }

        public void ToggleOverlayInfo()
        {
            if (InputA != null)
            {
                if (InputA.mi == null) InputA.mi = new MediaInfoDotNet.MediaFile(InputA.Info.FullName);
                if (InputA.mi != null) { OverlayInfoA.Text = InputA.mi.Inform; } else { OverlayInfoA.Text = "No data."; }
            }
            if (InputB != null)
            {
                if (InputB.mi == null) InputB.mi = new MediaInfoDotNet.MediaFile(InputB.Info.FullName);
                if (InputB.mi != null) { OverlayInfoB.Text = InputB.mi.Inform; } else { OverlayInfoB.Text = "No data."; }
            }
            if (OverlayFadeA.Visibility == Visibility.Hidden)
            {
                OverlayFadeA.Visibility = Visibility.Visible;
                OverlayFadeB.Visibility = Visibility.Visible;
            }
            else
            {
                OverlayFadeA.Visibility = Visibility.Hidden;
                OverlayFadeB.Visibility = Visibility.Hidden;
            }
        }

        private void FFMainWindow_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Escape) ButtonPlayA.Focus();
            
            if (ResizeVideoHeight.IsFocused || TextBoxExtraArgsAudio.IsFocused || TextBoxExtraArgsVideo.IsFocused || TextBoxExtraArgsVideoFilter.IsFocused ||
                ResizeVideoWidth.IsFocused  || TextBoxAudioFileName.IsFocused  || TextBoxLUTFileName.IsFocused    || TextBoxGamma.IsFocused ||
                TextBoxTCh.IsFocused        || TextBoxTCm.IsFocused            || TextBoxTCs.IsFocused            || TextBoxTCf.IsFocused)
                return;

            switch(e.Key)
            {
                case Key.F1:
                    ToggleOverlayHelp();
                    e.Handled = true;
                    break;
                case Key.F2:
                    ToggleOverlayInfo();
                    e.Handled = true;
                    break;
                case Key.F3:
                    if (RightColumn.Width.Value < 1)
                    {
                        ShowRightPlayer();
                    }
                    else
                    {
                        HideRightPlayer();
                    }
                    e.Handled = true;
                    break;
                case Key.Left:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        //MediaA.Position = MediaA.Position - TimeSpan.FromSeconds(1.0);
                        MediaA.Position = MediaA.Position.Subtract(TimeSpan.FromSeconds(MediaA.PositionStep.TotalSeconds * MediaA.VideoFrameRate));
                    }
                    else
                    {
                        //await MediaA.StepBackward();
                        MediaA.Position = MediaA.Position - TimeSpan.FromSeconds(1.0 / MediaA.VideoFrameRate);
                    }
                    e.Handled = true;
                    break;
                case Key.Right:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        //MediaA.Position = MediaA.Position + TimeSpan.FromSeconds(1.0);
                        MediaA.Position = MediaA.Position.Add(TimeSpan.FromSeconds(MediaA.PositionStep.TotalSeconds * MediaA.VideoFrameRate));
                    }
                    else
                    {
                        if (MediaA.RemainingDuration.HasValue && MediaA.RemainingDuration.Value.TotalSeconds > 0)
                        {
                            //await MediaA.StepForward();
                            MediaA.Position =  MediaA.Position + TimeSpan.FromSeconds(1.0 / MediaA.VideoFrameRate);
                        }
                    }
                    e.Handled = true;
                    break;
                case Key.R:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        RenderFinal(sendToQueue: false);
                    }
                    else
                    {
                        Render(isPreview: true);
                    }
                    e.Handled = true;
                    break;
                case Key.Space:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        ButtonPlay_Click(ButtonPlayB, new RoutedEventArgs());
                    }
                    else
                    {
                        ButtonPlay_Click(ButtonPlayA, new RoutedEventArgs());
                    }
                    e.Handled = true;
                    break;
                case Key.H:
                    ShowRenderHistory();
                    e.Handled = true;
                    break;
                case Key.D1:
                case Key.NumPad1:
                    SetZoomLevel(-1.0, resetTranslate:true);
                    e.Handled = true;
                    break;
                case Key.D2:
                case Key.NumPad2:
                    SetZoomLevel(0.5, zoomOffset: 0);
                    e.Handled = true;
                    break;
                case Key.D3:
                case Key.NumPad3:
                    SetZoomLevel(1.0, zoomOffset: 0);
                    e.Handled = true;
                    break;
                case Key.D4:
                case Key.NumPad4:
                    SetZoomLevel(2.0, zoomOffset: 0);
                    e.Handled = true;
                    break;
                case Key.I:
                    ButtonSetInPoint.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    e.Handled = true;
                    break;
                case Key.O:
                    ButtonSetOutPoint.RaiseEvent(new RoutedEventArgs(Button.ClickEvent));
                    e.Handled = true;
                    break;
                default: break;
            }
            //Debug.WriteLine(e.Key.ToString());
        }

        private void SetZoomLevel(double zoom, bool resetTranslate=false, double zoomOffset=-1)
        {
            if (zoom < 0)
            {
                ZoomLevelOffset = 0;
                //ZoomLevelTarget = 1.0;
                ZoomLevelTarget = ZoomLevelFit;
            }
            else
            {
                if (zoomOffset < 0)
                    ZoomLevelOffset = zoom - ZoomLevelFit;
                else
                    ZoomLevelOffset = zoomOffset;
                ZoomLevelTarget = zoom;
            }
            if (resetTranslate) TranslateOffsetA = TranslateOffsetB = new Point(0, 0);
            CenterVideo();
        }

        private void ButtonShowLog_Click(object sender, RoutedEventArgs e)
        {
            ToggleShowLog(ToggleShowLog_Value);
        }

        private void ToggleShowLog(bool show)
        {
            if (show == false)
            {
                LogBar.Height = new GridLength(0, GridUnitType.Star);
            }
            else
            {
                LogBar.Height = new GridLength(1, GridUnitType.Star);
            }
            GridSplitView.InvalidateVisual();
            GridSplitView.UpdateLayout();
            CenterVideo();
        }

        private void ButtonClearLog_Click(object sender, RoutedEventArgs e)
        {
            LogOutput.Clear();
            TheLog = "";
        }

        private void ButtonCopyLog_Click(object sender, RoutedEventArgs e)
        {
            Clipboard.SetText(TheLog);
        }

        private void ButtonRenderFinal_Click(object sender, RoutedEventArgs e)
        {
            RenderFinal(sendToQueue: false);
        }


        private void NumbersOnly_PreviewTextInput(object sender, TextCompositionEventArgs e)
        {
            e.Handled = !Helpers.IsTextAllowed(e.Text);
        }

        private void NumbersOnly_Pasting(object sender, DataObjectPastingEventArgs e)
        {
            if (e.DataObject.GetDataPresent(typeof(String)))
            {
                String text = (String)e.DataObject.GetData(typeof(String));
                if (!Helpers.IsTextAllowed(text))
                {
                    e.CancelCommand();
                }
            }
            else
            {
                e.CancelCommand();
            }
        }


        private void ResizeVideoWidth_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var o = sender as TextBox;
            int result;

            switch (e.Key)
            {
                case Key.Up:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        if (int.TryParse(o.Text, out result))
                        {
                            o.Text = (result + 10).ToString();
                        }
                    }
                    else
                    {
                        if (int.TryParse(o.Text, out result))
                        {
                            o.Text = (result + 1).ToString();
                        }
                    }
                    break;
                case Key.Down:
                    if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift)
                    {
                        if (int.TryParse(o.Text, out result))
                        {
                            o.Text = (result - 10).ToString();
                        }
                    }
                    else
                    {
                        if (int.TryParse(o.Text, out result))
                        {
                            o.Text = (result - 1).ToString();
                        }
                    }
                    break;
                case Key.Return:
                case Key.Escape:
                    LogOutput.Focus();
                    break;
                default:
                    break;
            }
        }

        private void ResizeVideoWidth_TextChanged(object sender, TextChangedEventArgs e)
        {
            var o = sender as TextBox;
            int result;
            if (int.TryParse(o.Text, out result))
            {
                CurrentSettings.Width = result;
                if (ToggleForceAspectRatio_Value && !dontUpdateAspectRatio)
                {
                    dontUpdateAspectRatio = true;
                    var ratio = Helpers.ConvertAspectRatioStringToDouble(MediaA.VideoAspectRatio);
                    CurrentSettings.Height = (int)Math.Floor(result / (ratio==null ? 1.0 : (double)ratio));
                    ResizeVideoHeight.Text = CurrentSettings.Height.ToString();
                    dontUpdateAspectRatio = false;
                }
            }
        }

        private void ResizeVideoHeight_TextChanged(object sender, TextChangedEventArgs e)
        {
            var o = sender as TextBox;
            int result;
            if (int.TryParse(o.Text, out result))
            {
                CurrentSettings.Height = result;
                if (ToggleForceAspectRatio_Value && !dontUpdateAspectRatio)
                {
                    dontUpdateAspectRatio = true;
                    var ratio = Helpers.ConvertAspectRatioStringToDouble(MediaA.VideoAspectRatio);
                    CurrentSettings.Width = (int)Math.Floor(result * (ratio == null ? 1.0 : (double)ratio));
                    ResizeVideoWidth.Text = CurrentSettings.Width.ToString();
                    dontUpdateAspectRatio = false;
                }
            }
        }

        private void CheckForceAspectRatio_Checked(object sender, RoutedEventArgs e)
        {
            var o = ResizeVideoWidth;
            int result;
            if (int.TryParse(o.Text, out result))
            {
                dontUpdateAspectRatio = true;
                var ratio = Helpers.ConvertAspectRatioStringToDouble(MediaA.VideoAspectRatio);
                CurrentSettings.Height = (int)Math.Floor(result / (ratio == null ? 1.0 : (double)ratio));
                ResizeVideoHeight.Text = CurrentSettings.Height.ToString();
                dontUpdateAspectRatio = false;
            }
        }

        private void RenderFinal(bool sendToQueue=false, RenderSettingsItem rs=null)
        {
            if (!sendToQueue)
            {
                if (!MediaA.IsOpen || !MediaA.HasVideo) return;
            }

            string fn = GetSaveFileName(rs);
            if (fn != null)
            {
                Render(addHistory: true, useRange: ToggleUseRangeForFinal_Value, outputPath: fn, isPreview: false, sendToQueue: sendToQueue);
            }
        }

        public string GetSaveFileName(RenderSettingsItem rs=null, FileMeta input=null, bool generateAutoFileName=false, string windowTitle=null)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            if (windowTitle != null) saveFileDialog.Title = windowTitle;
            string fn;
            if (rs != null)
            {
                saveFileDialog.InitialDirectory = Path.GetFileName(rs.OutputPath);
                fn = Path.GetFileName(rs.OutputPath);
            }
            else
            {
                if (rs == null)
                {
                    rs = CurrentSettings;
                    if (input == null) input = InputA;
                }
                else
                {
                    if (input == null)
                    {
                        //input = new FileMeta(rs.InputFile.FileInfo);
                        input = FileMeta.Create(rs.InputFile.FileInfo.FullName);
                    }
                }

                fn = input.BaseFileName;
                if (rs.EncoderVideo.Name == VideoCodec.None && rs.EncoderAudio.Name != AudioCodec.None)
                {
                    fn = fn + "_" + rs.EncoderAudio.Name + rs.EncoderAudio.FileExtension;
                    saveFileDialog.Filter = $"{rs.EncoderAudio.Name} ({rs.EncoderAudio.FileExtension})|*{rs.EncoderAudio.FileExtension}|All Files (*.*)|*.*";
                }
                else if (rs.EncoderVideo.Name != VideoCodec.None)
                {
                    fn = fn + "_" + rs.EncoderVideo.Name + rs.EncoderVideo.FileExtension;
                    saveFileDialog.Filter = $"{rs.EncoderVideo.Name} ({rs.EncoderVideo.FileExtension})|*{rs.EncoderVideo.FileExtension}|All Files (*.*)|*.*";
                }
                else
                {
                    fn = fn + "_encoded";
                }
                if (generateAutoFileName) return fn;
            }

            saveFileDialog.FileName = fn;

            if (saveFileDialog.ShowDialog() == true)
            {
                return saveFileDialog.FileName;
            }
            return null;
        }

        private void ComboInputFps_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            CurrentFrameRateEntry = ((sender as ComboBox).SelectedItem) as FrameRates.FrameRateEntry;
            UpdateMediaSources(onlyLeftSide: true, forceReload: true);
        }

        private void ToggleMoreVideo_Checked(object sender, RoutedEventArgs e)
        {
            VideoBar2.Height = new GridLength(30);
        }

        private void ToggleMoreVideo_Unchecked(object sender, RoutedEventArgs e)
        {
            VideoBar2.Height = new GridLength(0);
        }

        private void ToggleMoreVideo2_Checked(object sender, RoutedEventArgs e)
        {
            VideoBar3.Height = new GridLength(30);
        }

        private void ToggleMoreVideo2_Unchecked(object sender, RoutedEventArgs e)
        {
            VideoBar3.Height = new GridLength(0);
        }

        private void ToggleMoreAudio_Checked(object sender, RoutedEventArgs e)
        {
            AudioBar2.Height = new GridLength(30);
        }

        private void ToggleMoreAudio_Unchecked(object sender, RoutedEventArgs e)
        {
            AudioBar2.Height = new GridLength(0);
        }

        private void ButtonLUTFileName_Click(object sender, RoutedEventArgs e)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Filter = "LUT3D (*.3dl;*.cube;*.dat;*.m3d)|*.3dl;*.cube;*.dat;*.m3d";
            if (openFileDialog.ShowDialog() == true)
            {
                TextBoxLUTFileName.Text = openFileDialog.FileName;
            }
        }

        private void TextBoxLUTFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (File.Exists(TextBoxLUTFileName.Text))
            {
                ToggleCheckApplyLUT_Value = true;
            }
            else
            {
                ToggleCheckApplyLUT_Value = false;
            }
        }

        private void ButtonShowPresets_Click(object sender, RoutedEventArgs e)
        {
            if (WinPresets == null || WinPresets.IsLoaded == false)
            {
                WinPresets = new WindowPresets(this);
            }
            WinPresets.Show();
            WinPresets.Activate();
        }

        private void Menu_SaveCurrentSettingsAsPreset_Click(object sender, RoutedEventArgs e)
        {
            //Render(addHistory: false, useRange: false, saveAsPresetOnly: true);
            SaveCurrentSettingsAsPreset();
        }

        private void Menu_PresetsRefresh_Click(object sender, RoutedEventArgs e)
        {
            ReadPresetsMenuItems(Menu_PresetsRoot);
        }

        private void Menu_LoadPresetsFromTag(object sender, RoutedEventArgs e)
        {
            var o = sender as MenuItem;
            var preset = o.Tag as RenderSettingsItem;
            LoadFromRenderSettings(preset);
        }

        private void ShowRenderHistory_Click(object sender, RoutedEventArgs e)
        {
            ShowRenderHistory();
        }


        private void Menu_Open_Recent_Click(object sender, RoutedEventArgs e)
        {
            MenuItem o = e.OriginalSource as MenuItem;
            if (o == null) return;
            else
            {
                OpenFile(o.DataContext.ToString());
            }
        }

        private void ButtonOpenStdFile_Click(object sender, RoutedEventArgs e)
        {
            OpenFileStandard();
        }

        private void SliderVolume_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double> e)
        {
            if (MediaA != null) MediaA.Volume = e.NewValue;
            if (MediaB != null) MediaB.Volume = e.NewValue;
        }

        private void TextBoxAudioFileName_TextChanged(object sender, TextChangedEventArgs e)
        {
            CheckAudioReplace.IsChecked = true;
        }

        private void ButtonShowQueue_Click(object sender, RoutedEventArgs e)
        {
            ShowRenderQueue();
        }

        private void ShowRenderQueue()
        {
            if (WinRenderQueue == null || WinRenderQueue.IsLoaded == false)
            {
                WinRenderQueue = new WindowRenderQueue(this);
                WinRenderQueue.DataGridRenderQueue.ItemsSource = this.RenderQueue;
            }
            WinRenderQueue.Show();
            WinRenderQueue.Activate();
            WinRenderQueue.RefreshItems();
        }

        private void ButtonSendToQueue_Click(object sender, RoutedEventArgs e)
        {
            RenderFinal(sendToQueue: true);
            ShowRenderQueue();
        }

        private void Close_File_Click(object sender, RoutedEventArgs e)
        {
            MediaA?.Stop();
            MediaB?.Stop();
            MediaA?.Close();
            MediaB?.Close();
        }

        private void ShowRenderHistory()
        {
            if (WinRenderHistory == null || WinRenderHistory.IsLoaded == false)
            {
                WinRenderHistory = new WindowRenderHistory(this);
                WinRenderHistory.DataGridRenderHistory.ItemsSource = this.RenderHistory;
            }
            WinRenderHistory.Show();
            WinRenderHistory.Activate();
        }

        public void LoadFromRenderSettings(RenderSettingsItem rh)
        {
            ToggleAutoPreview_Value = false;
            CurrentSettings = rh.GetClone();

            if (rh.SourcePath != null) InputA = FileMeta.Create(rh.SourcePath);
            if (rh.OutputPath != null) InputB = FileMeta.Create(rh.OutputPath);

            if (!VideoCodec.SettingsCollection.Contains(rh.EncoderVideo.Name))
            {
                rh.EncoderVideo.Name = $"Preset:{rh.EncoderVideo.Name}";
                VideoCodec.SettingsCollection.Add(rh.EncoderVideo);
            }
            ComboEncoderVideo.SelectedItem = VideoCodec.SettingsCollection[rh.EncoderVideo.Name];

            if (!AudioCodec.SettingsCollection.Contains(rh.EncoderAudio.Name))
            {
                rh.EncoderAudio.Name = $"Preset:{rh.EncoderAudio.Name}";
                AudioCodec.SettingsCollection.Add(rh.EncoderAudio);
            }
            ComboEncoderAudio.SelectedItem = AudioCodec.SettingsCollection[rh.EncoderAudio.Name];

            if (rh.Color_Range_In == "pc") ComboBoxColorRangeIn.SelectedIndex = 1;
            else if (rh.Color_Range_In == "tv") ComboBoxColorRangeIn.SelectedIndex = 2;
            else ComboBoxColorRangeIn.SelectedIndex = 0;

            if (rh.Color_Range_Out == "pc") ComboBoxColorRangeOut.SelectedIndex = 1;
            else if (rh.Color_Range_Out == "tv") ComboBoxColorRangeOut.SelectedIndex = 2;
            else ComboBoxColorRangeOut.SelectedIndex = 0;

            TextBoxExtraArgsVideoFilter.Text = rh.ExtraVideoFilters;

            SliderQualityAudio.Value = rh.QualityAudio;
            SliderQualityVideo.Value = rh.QualityVideo;
            ResizeVideoWidth.Text = rh.Width.ToString();
            ResizeVideoHeight.Text = rh.Height.ToString();
            ToggleResizeVideo_Value = rh.ResizeVideo;
            ComboAudioSampleRate.SelectedItem = rh.AudioSampleRate;
            ToggleReplaceAudio_Value = rh.ReplaceAudio;
            TextBoxAudioFileName.Text = rh.ReplaceAudioFileName;
            TextBoxLUTFileName.Text = rh.LUT;
            ToggleCheckBurnInTimeCode_Value = rh.BurnTimeCode;
            ToggleCheckSetTimeCode_Value = rh.SetTimeCode;
            ToggleCheckApplyLUT_Value = rh.ApplyLUT;
            TextBoxTCh.Text = rh.TCh;
            TextBoxTCm.Text = rh.TCm;
            TextBoxTCs.Text = rh.TCs;
            TextBoxTCf.Text = rh.TCf;
            UpdatePixFmts();

            if (rh.MediaType == MediaType.ImageSequence || rh.MediaType == MediaType.Image)
            {
                ComboInputFpsLabel.Visibility = ComboInputFps.Visibility = Visibility.Visible;
                ComboInputFps.IsEnabled = true;
                ComboInputFps.SelectedItem = rh.FrameRate;
            }
            else
            {
                ComboInputFps.IsEnabled = false;
                ComboInputFpsLabel.Visibility = ComboInputFps.Visibility = Visibility.Hidden;
            }

            

            UpdateMediaSources();
        }
    }
}
