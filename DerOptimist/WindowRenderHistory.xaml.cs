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
using Newtonsoft.Json;
using Path = System.IO.Path;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for WindowRenderHistory.xaml
    /// </summary>
    public partial class WindowRenderHistory
    {
        MainWindow parent;
        public WindowRenderHistory(MainWindow parent)
        {
            InitializeComponent();
            DataContext = parent;
            this.parent = parent;
            RSControl.ShowInstanceDetails();
            KeyDown += DataGridRenderHistory_KeyDown;
        }

        private void MenuItemOpen_Click(object sender, RoutedEventArgs e)
        {

        }

        private void ButtonDeleteSelected_Click(object sender, RoutedEventArgs e)
        {
            //parent.DeletePreviewFromHistory
            var selected = parent.RenderHistory.Where(o => o.IsSelected == true).ToList();
            foreach (var item in selected)
            {
                parent.DeletePreviewFromHistory(item.guid);
            }
            RefreshItems();
        }

        private void MenuItem_Delete_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                parent.DeletePreviewFromHistory(item.guid);
            }
            RefreshItems();
        }

        private void DataGridRenderHistory_SelectedCellsChanged(object sender, SelectedCellsChangedEventArgs e)
        {
            Debug.WriteLine(e.AddedCells);
            RSControl.GridRenderSettings.DataContext = DataGridRenderHistory.SelectedItem as RenderSettingsItem;
        }

        private void DataGridRenderHistory_KeyDown(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Space)
            {
                foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
                {
                    item.IsSelected = !item.IsSelected;
                }
                RefreshItems();
            }
        }

        private void MenuItem_Select_Click(object sender, RoutedEventArgs e)
        {
            foreach(RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
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

        private void MenuItem_ExplorerSelectPreview_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Process.Start("explorer.exe", $"/select,\"{item.OutputPath}\"");
            }
        }

        private void Button_OpenPreview_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Process.Start($"{item.OutputPath}");
            }
        }

        private void MenuItem_ExplorerSelectSource_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Process.Start("explorer.exe", $"/select,\"{item.SourcePath}\"");
            }
        }

        private void Button_OpenSource_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Process.Start($"{item.SourcePath}");
            }
        }

        public void RefreshItems()
        {
            DataGridRenderHistory.Items.Refresh();
        }

        private void DataGridRenderHistory_MouseDoubleClick(object sender, MouseButtonEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                parent.LoadFromRenderSettings(item);
            }
        }

        private void MenuItem_CopyFfmpeg_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Clipboard.SetText(item.FfmpegParameters);
            }
        }

        private void MenuItem_CopyOutputPath_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Clipboard.SetText(item.OutputPath);
            }
        }

        private void MenuItem_CopySourcePath_Click(object sender, RoutedEventArgs e)
        {
            foreach (RenderSettingsItem item in DataGridRenderHistory.SelectedItems)
            {
                Clipboard.SetText(item.SourcePath);
            }
        }

        private void ButtonSaveAsPreset_Click(object sender, RoutedEventArgs e)
        {
            if (DataGridRenderHistory.SelectedItem != null)
            {
                var item = DataGridRenderHistory.SelectedItem as RenderSettingsItem;
                RenderSettingsItem.ToJsonFileWithDialog(item);
                parent.ReadPresetsMenuItems(parent.Menu_PresetsRoot);
            }
        }
    }
}
