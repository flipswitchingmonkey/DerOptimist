using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Shapes;
using Path = System.IO.Path;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for WindowPreferences.xaml
    /// </summary>
    public partial class WindowPreferences : Window
    {
        MainWindow parent;
        bool PrefsHaveChanged = false;

        public WindowPreferences(MainWindow parent)
        {
            this.parent = parent;
            InitializeComponent();
            PrefsHaveChanged = false;  // to make sure the initialization has not triggered any pseudo-change
        }

        private void ButtonFfmpegBinaryPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.OpenFileDialog();
            try
            {
                var initial = Path.GetFullPath(TxtBoxFfmpegBinaryPath.Text);
                dialog.InitialDirectory = Path.GetDirectoryName(initial);
            }
            catch
            {
                dialog.InitialDirectory = "";
            }
            dialog.Title = "Select ffmpeg.exe binary";
            dialog.Filter = "ffmpeg Executable|*.exe"; 
            dialog.FileName = "ffmpeg.exe";
            var result = dialog.ShowDialog();
            if (result == true) {
                string path = dialog.FileName;
                if (File.Exists(path))
                {
                    Properties.Settings.Default.ffmpegBinaryPath = path;
                    PrefsHaveChanged = true;
                }
            }
        }

        private void ButtonDefaultOutputPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            try
            {
                var initial = Path.GetFullPath(TxtBoxDefaultOutputPath.Text);
                dialog.InitialDirectory = Path.GetDirectoryName(initial);
            }
            catch
            {
                dialog.InitialDirectory = "";
            }
            dialog.Title = "Select default render output directory";
            dialog.Filter = "Directory|*.this.directory"; 
            dialog.FileName = "select";
            var result = dialog.ShowDialog();
            if (result == true) {
                var path = dialog.FileName;
                var outputDir = Path.GetDirectoryName(Path.GetFullPath(path));
                if (Directory.Exists(outputDir))
                {
                    Properties.Settings.Default.defaultOutputPath = outputDir;
                    PrefsHaveChanged = true;
                }
            }
        }

        private void ButtonDefaultPreviewPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            try
            {
                var initial = Path.GetFullPath(TxtBoxDefaultPreviewPath.Text);
                dialog.InitialDirectory = Path.GetDirectoryName(initial);
            }
            catch
            {
                dialog.InitialDirectory = "";
            }
            dialog.Title = "Select default preview output directory";
            dialog.Filter = "Directory|*.this.directory"; 
            dialog.FileName = "select";
            var result = dialog.ShowDialog();
            if (result == true) {
                var path = dialog.FileName;
                var outputDir = Path.GetDirectoryName(Path.GetFullPath(path));
                if (Directory.Exists(outputDir))
                {
                    Properties.Settings.Default.defaultPreviewPath = outputDir;
                    PrefsHaveChanged = true;
                }
            }
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            if (PrefsHaveChanged == true)
            {
                var result = MessageBox.Show("Preferences have changed, save changes?", "Save Preferences", MessageBoxButton.YesNoCancel);
                if (result == MessageBoxResult.Cancel)
                {
                    e.Cancel = true;
                }
                else if (result == MessageBoxResult.Yes)
                {
                    Properties.Settings.Default.Save();
                }
                else {
                    Properties.Settings.Default.Reload();
                }
            }
            
        }

        private void ButtonPrefsSave_Click(object sender, RoutedEventArgs e)
        {
            Properties.Settings.Default.Save();
            PrefsHaveChanged = false;
            this.Close();
        }

        private void ComboDefaultEncoderVideo_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboDefaultEncoderVideo.SelectedItem == null) return; 
            Properties.Settings.Default.defaultEncoderVideo = (ComboDefaultEncoderVideo.SelectedItem as FFmpeg.NET.Enums.VideoCodecEntry).Name;
            PrefsHaveChanged = true;
        }

        private void NumericPreviewDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            Properties.Settings.Default.defaultPreviewDuration = (double)NumericPreviewDuration.Value;
            PrefsHaveChanged = true;
        }

        private void CheckKeepPreviewFiles_Checked(object sender, RoutedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void CheckKeepPreviewFiles_Unchecked(object sender, RoutedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void CheckKeepPreviewFilesHistoryDelete_Checked(object sender, RoutedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void CheckKeepPreviewFilesHistoryDelete_Unchecked(object sender, RoutedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void NumericDefaultFrameRate_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            PrefsHaveChanged = true;
        }

        private void NumericSingleImageDuration_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            PrefsHaveChanged = true;
        }

        private void TxtBoxExtensionsVideo_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void TxtBoxExtensionsAudio_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void TxtBoxExtensionsImage_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void ComboDefaultEncoderAudio_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (ComboDefaultEncoderAudio.SelectedItem == null) return;
            Properties.Settings.Default.defaultEncoderAudio = (ComboDefaultEncoderAudio.SelectedItem as FFmpeg.NET.Enums.AudioCodecEntry).Name;
            PrefsHaveChanged = true;
        }

        private void NumericSingleTimeCodeFontSize_ValueChanged(object sender, RoutedPropertyChangedEventArgs<double?> e)
        {
            PrefsHaveChanged = true;
        }

        private void ComboTimeCodeColor_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void ButtonPresetFolderPath_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new Microsoft.Win32.SaveFileDialog();
            try
            {
                var initial = Path.GetFullPath(TxtBoxPresetFolderPath.Text);
                dialog.InitialDirectory = Path.GetDirectoryName(initial);
            }
            catch
            {
                dialog.InitialDirectory = "";
            }
            dialog.Title = "Select preset files directory";
            dialog.Filter = "Directory|*.this.directory";
            dialog.FileName = "select";
            var result = dialog.ShowDialog();
            if (result == true)
            {
                var path = dialog.FileName;
                var outputDir = Path.GetDirectoryName(Path.GetFullPath(path));
                if (Directory.Exists(outputDir))
                {
                    Properties.Settings.Default.PresetFolderPath = outputDir;
                    PrefsHaveChanged = true;
                }
            }
        }

        private void TxtBoxPresetFolderPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void TxtBoxDefaultPreviewPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }

        private void TxtBoxDefaultOutputPath_TextChanged(object sender, TextChangedEventArgs e)
        {
            PrefsHaveChanged = true;
        }
    }
}
