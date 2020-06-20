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
using System.Windows.Forms;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.IO;
using TextBox = System.Windows.Controls.TextBox;
using System.Diagnostics;
using System.Runtime.Serialization.Formatters.Binary;
using Path = System.IO.Path;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for SequenceDialog.xaml
    /// </summary>
    public partial class SequenceDialog
    {
        FileList Entries;
        List<Location> Locations;
        List<Bookmark> Bookmarks;
        FileMeta SelectedFile;
        MainWindow ParentWindow;
        String CurrentDirectory;

        bool _initializing;

        public SequenceDialog(MainWindow parent, string directory)
        {
            _initializing = true;
            ParentWindow = parent;
            InitializeComponent();
            InitializeValues();
            InitializeEntries(directory);
            InitializeLocations();
            InitializeBookmarks();
            _initializing = false;
        }

        private void InitializeValues()
        {
            DataContext = this;
            SelectedFile = null;
        }

        private void InitializeEntries(string directory)
        {
            CurrentDirectory = directory;
            Entries = new FileList(CurrentDirectory);
            RefreshEntries();
            TextBoxDirectory.Text = CurrentDirectory;
        }

        private void InitializeLocations()
        {
            RefreshLocations();
        }

        private void InitializeBookmarks()
        {
            //Bookmarks = new List<Bookmark>();
            Bookmarks = LoadBookmarks();
            RefreshBookmarks();
        }

        private void RefreshLocations()
        {
            DataGridLocations.ItemsSource = null;
            Locations = new List<Location>();
            DriveInfo[] allDrives = DriveInfo.GetDrives();
            foreach (DriveInfo driveInfo in allDrives)
            {
                Debug.WriteLine("Drive {0}", driveInfo.Name);
                Debug.WriteLine("  File type: {0}", driveInfo.DriveType);
                Locations.Add(new Location(driveInfo));
            }
            
            DataGridLocations.ItemsSource = Locations;
        }

        private void RefreshEntries()
        {
            if (Entries == null) return;
            DataGridSequences.ItemsSource = null;
            Entries.Refresh(GetFilter());
            var dirInfo = new DirectoryInfo(CurrentDirectory);
            if (dirInfo.Parent != null)
            {
                var up = new FileMeta()
                {
                    MediaType = MediaType.Up,
                    FAIcon = MediaIcons.Up
                };
                Entries.Insert(0, up);
            }
            DataGridSequences.ItemsSource = Entries.meta;
        }

        private void RefreshBookmarks()
        {
            DataGridBookmarks.ItemsSource = null;
            //Entries.Refresh();
            DataGridBookmarks.ItemsSource = Bookmarks;
        }

        private void TextBoxDirectory_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initializing) return;
            var t = sender as TextBox;
            string newText = t.Text;
            if (Directory.Exists(newText))
            {
                t.Foreground = Brushes.Black;
                InitializeEntries(newText);
            }
            else
            {
                t.Foreground = Brushes.Red;
            }
        }

        private void ButtonOpenDir_Click(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.FolderBrowserDialog();
            dialog.SelectedPath = TextBoxDirectory.Text;
            System.Windows.Forms.DialogResult result = dialog.ShowDialog();
            if (result == System.Windows.Forms.DialogResult.OK)
            {
                SetCurrentDirectory(dialog.SelectedPath);
            }
        }

        private void ButtonParentDir_Click(object sender, RoutedEventArgs e)
        {
            var dir = TextBoxDirectory.Text;
            if (Directory.Exists(dir))
            {
                SetCurrentDirectory(Directory.GetParent(dir).ToString());
            }
        }

        private void CheckShowSequences_Checked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            if (Entries == null) return;
            Entries.GroupSequences = true;
            RefreshEntries();
        }

        private void CheckShowSequences_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            if (Entries == null) return;
            Entries.GroupSequences = false;
            RefreshEntries();
        }

        private void CheckSplitSequenceIfMissingFrames_Checked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            if (Entries == null) return;
            Entries.SplitSequenceIfMissingFrames = true;
            RefreshEntries();
        }

        private void CheckSplitSequenceIfMissingFrames_Unchecked(object sender, RoutedEventArgs e)
        {
            if (_initializing) return;
            if (Entries == null) return;
            Entries.SplitSequenceIfMissingFrames = false;
            RefreshEntries();
        }

        private void ButtonAddBookmark_Click(object sender, RoutedEventArgs e)
        {
            var cd = GetCurrentDirectory();
            if (cd != null)
            {
                Bookmarks.Add(new Bookmark(cd.FullName));
                RefreshBookmarks();
                SaveBookmarks(Bookmarks);
            }
        }

        private void ButtonRemoveBookmark_Click(object sender, RoutedEventArgs e)
        {
            var bm = DataGridBookmarks.SelectedItem as Bookmark;
            if (bm != null)
            {
                Bookmarks.Remove(bm);
                RefreshBookmarks();
                SaveBookmarks(Bookmarks);
            }
        }

        void SaveBookmarks(List<Bookmark> bookmarks)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                BinaryFormatter bf = new BinaryFormatter();
                bf.Serialize(ms, bookmarks);
                ms.Position = 0;
                byte[] buffer = new byte[(int)ms.Length];
                ms.Read(buffer, 0, buffer.Length);
                Properties.Settings.Default.Bookmarks = Convert.ToBase64String(buffer);
                Properties.Settings.Default.Save();
            }
        }

        List<Bookmark> LoadBookmarks()
        {
            if (Properties.Settings.Default.Bookmarks != null && Properties.Settings.Default.Bookmarks != "")
            {
                using (MemoryStream ms = new MemoryStream(Convert.FromBase64String(Properties.Settings.Default.Bookmarks)))
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    return (List<Bookmark>)bf.Deserialize(ms);
                }
            }
            return new List<Bookmark>();
        }

        private DirectoryInfo GetCurrentDirectory()
        {
            string directory = TextBoxDirectory.Text;
            if (Directory.Exists(directory))
            {
                return new DirectoryInfo(directory);
            }
            return null;
        }

        private void SetCurrentDirectory(string directory)
        {
            if (Directory.Exists(directory))
            {
                var di = new DirectoryInfo(directory);
                TextBoxDirectory.Text = di.FullName;
            }
        }

        private void DataGridSequences_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            OpenSelectedFile();
        }


        private void DataGridSequences_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var fm = DataGridSequences.SelectedItem as FileMeta;
            if (fm != null)
            {
                if (fm.MediaType == MediaType.Directory)
                {
                    //SetCurrentDirectory(fm.Info.FullName);
                }
                else if (fm.MediaType == MediaType.Up)
                {
                    //SetCurrentDirectory(Directory.GetParent(CurrentDirectory).ToString());
                }
                else
                {
                    SelectedFile = fm;
                    TextBoxFile.Text = SelectedFile.Info.Name;
                }
            }
        }

        private void DataGridBookmarks_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            var bm = DataGridBookmarks.SelectedItem as Bookmark;
            if (bm != null)
            {
                SetCurrentDirectory(bm.Info.FullName);
            }
        }

        private void DataGridLocations_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            var loc = DataGridLocations.SelectedItem as Location;
            if (loc != null)
            {
                SetCurrentDirectory(loc.Info.RootDirectory.FullName);
            }
        }

        private void TextBoxFile_TextChanged(object sender, TextChangedEventArgs e)
        {
            if (_initializing) return;
            if (CheckFileExists(TextBoxFile.Text))
            {
                TextBoxFile.Foreground = Brushes.Black;
            }
            else
            {
                TextBoxFile.Foreground = Brushes.Red;
            }
        }

        private bool CheckFileExists(string fileName)
        {
            string path = Path.Combine(GetCurrentDirectory().FullName, fileName);
            if (File.Exists(path))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private void ButtonOpenFile_Click(object sender, RoutedEventArgs e)
        {
            OpenSelectedFile();
        }

        private void SaveLastOpenedDirectory()
        {

        }

        private void OpenSelectedFile()
        {
            var fm = DataGridSequences.SelectedItem as FileMeta;
            if (fm != null)
            {
                if (fm.MediaType == MediaType.Directory)
                {
                    SetCurrentDirectory(fm.Info.FullName);
                }
                else if (fm.MediaType == MediaType.Up)
                {
                    SetCurrentDirectory(Directory.GetParent(CurrentDirectory).ToString());
                }
                else
                {
                    SelectedFile = fm;
                    TextBoxFile.Text = SelectedFile.Info.Name;
                    var info = fm.Info as FileInfo;
                    ParentWindow.OpenFile(info.FullName);
                    CurrentDirectory = info.DirectoryName;
                    this.Close();
                }
            }
        }

        private void MetroWindow_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Properties.Settings.Default.lastOpenedDirectory = CurrentDirectory;
            Properties.Settings.Default.Save();
        }

        private void ButtonRefreshDir_Click(object sender, RoutedEventArgs e)
        {
            RefreshEntries();
        }

        private MediaType GetFilter()
        {
            switch (FilterEntries.SelectedValue as string)
            {
                case "AllMedia": return MediaType.All;
                case "Video": return MediaType.Movie;
                case "Audio": return MediaType.Audio;
                case "Images": return MediaType.Image;
                case "AllFiles": return MediaType.Unknown;
                default: return MediaType.All;
            }
            
        }

        private void FilterEntries_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            RefreshEntries();
        }
    }
}
