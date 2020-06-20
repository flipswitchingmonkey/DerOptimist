using System;
using System.Collections.Generic;
using System.Diagnostics;
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
using FFmpeg.NET;
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for WindowRenderHistory.xaml
    /// </summary>
    public partial class WindowRenderQueue
    {
        MainWindow parent;
        public WindowRenderQueue(MainWindow parent)
        {
            InitializeComponent();
            DataContext = parent;
            this.parent = parent;
            //RSControl.ShowInstanceDetails();
            KeyDown += DataGridRenderQueue_KeyDown;
            this.RSControl.parentQueue = this;
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            //parent.DeletePreviewFromHistory
            var selected = parent.RenderHistory.Where(o => o.IsSelected == true).ToList();
            if (selected.Count == 0)
            {
                selected.Add((e.OriginalSource as Button).DataContext as RenderSettingsItem);
            }
            foreach (var item in selected)
            {
                if (item.cancellationTokenSource != null && item.Status == RenderQueueItemStatus.Encoding) item.cancellationTokenSource.Cancel();
                parent.DeleteQueueItemFromHistory(item.guid);
            }
            RefreshItems();
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                parent.DeleteQueueItemFromHistory(item.guid);
            }
            RefreshItems();
        }

        private void DataGridRenderQueue_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Debug.WriteLine(e.AddedCells);
            RSControl.GridRenderSettings.DataContext = DataGridRenderQueue.SelectedItem as RenderSettingsItem;
        }

        private void DataGridRenderQueue_KeyDown(object sender, KeyEventArgs e)
        {
            //if (e.Key == Key.Space)
            //{
            //    foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            //    {
            //        item.IsSelected = !item.IsSelected;
            //    }
            //    RefreshItems();
            //}
        }

        private void MenuItem_Select_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                item.IsSelected = !item.IsSelected;
            }
            RefreshItems();
        }

        private void DataGridCell_PreviewMouseDown(object sender, MouseButtonEventArgs e)
        {
            var cell = sender as DataGridCell;
            var column = cell.Column.Header.ToString();


            if (column == "Selected")
            {
                var row = Helpers.GetVisualParent<DataGridRow>(e.OriginalSource as DependencyObject);
                if (row != null)
                {
                    var item = row.Item as RenderSettingsItem;
                    item.IsSelected = !item.IsSelected;
                    RefreshItems();
                }
            }
        }

        //private void MenuItem_ExplorerSelectPreview_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
        //    {
        //        Process.Start("explorer.exe", $"/select,\"{item.OutputPath}\"");
        //    }
        //}

        //private void Button_OpenPreview_Click(object sender, RoutedEventArgs e)
        //{
        //    foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
        //    {
        //        Process.Start($"{item.OutputPath}");
        //    }
        //}

        private void MenuItem_ExplorerSelectSource_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                Process.Start("explorer.exe", $"/select,\"{item.SourcePath}\"");
            }
        }

        private void Button_OpenSource_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                Process.Start($"{item.SourcePath}");
            }
        }

        public void RefreshItems()
        {
            DataGridRenderQueue.Items.Refresh();
        }

        private void DataGridRenderQueue_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                //parent.LoadFromRenderSettings(item);
            }
        }

        private void MenuItem_CopyFfmpeg_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                Clipboard.SetText(item.FfmpegParameters);
            }
        }

        private void MenuItem_CopyOutputPath_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                Clipboard.SetText(item.OutputPath);
            }
        }

        private void MenuItem_CopySourcePath_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                Clipboard.SetText(item.SourcePath);
            }
        }

        //private void ButtonSaveAsPreset_Click(object sender, RoutedEventArgs e)
        //{
        //    if (DataGridRenderQueue.SelectedItem != null)
        //    {
        //        var item = DataGridRenderQueue.SelectedItem as RenderSettingsItem;
        //        RenderSettingsItem.ToJsonFileWithDialog(item);
        //        parent.ReadPresetsMenuItems(parent.Menu_PresetsRoot);
        //    }
        //}

        private void RenderQueueItem_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridRenderQueue.SelectedItem != null)
            {
                var item = DataGridRenderQueue.SelectedItem as RenderSettingsItem;
                ActionItem(item);
            }
        }

        public void ActionItem(RenderSettingsItem item)
        {
            switch (item.Status)
            {
                case RenderQueueItemStatus.Queued:
                    parent.RenderFromRenderSettings(item);
                    break;
                case RenderQueueItemStatus.Encoding:
                    item.cancellationTokenSource.Cancel();
                    item.Status = RenderQueueItemStatus.Stopped;
                    break;
                case RenderQueueItemStatus.Finished:
                    Process.Start("explorer.exe", $"/select,\"{item.OutputPath}\"");
                    break;
                case RenderQueueItemStatus.Error:
                    if (item.Msg != null) MessageBox.Show(item.Msg, "Error", MessageBoxButton.OK);
                    break;
            }
            RefreshItems();
        }

        public void MenuItem_ChangeOutputPath_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderQueue.SelectedItems)
            {
                string fn = parent.GetSaveFileName(item, windowTitle:"Change Output Path");
                if (fn != null)
                {
                    item.OutputPath = fn;
                    item.OutputFile = new MediaFile(Path.Combine(item.OutputPath));
                }
                this.RefreshItems();
            }
        }

        private void ButtonEncodeAll_Click(object sender, RoutedEventArgs e)
        {
            parent.RenderAllQueued();
        }

        private void MenuItem_StartEncoding_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridRenderQueue.SelectedItem != null)
            {
                var item = DataGridRenderQueue.SelectedItem as RenderSettingsItem;
                ActionItem(item);
            }
        }

        private void DataGridRenderQueue_DragOver(object sender, DragEventArgs e)
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

        private void DataGridRenderQueue_Drop(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                
                string[] files = (string[])e.Data.GetData(DataFormats.FileDrop);
                foreach (var f in files) {
                    if (files.Length > 0)
                    {
                        parent.DirectToQueue(f);
                    }
                }
            }
            e.Effects = DragDropEffects.None;
        }

        private void DataGridCellOutputPath_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            if (DataGridRenderQueue.SelectedItem != null)
            {
                var item = DataGridRenderQueue.SelectedItem as RenderSettingsItem;
                MenuItem_ChangeOutputPath_Click(sender, e);
            }
        }
    }
}
