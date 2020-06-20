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
using Path = System.IO.Path;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for WindowPresets.xaml
    /// </summary>
    public partial class WindowPresets : Window
    {
        MainWindow parent;
        Preset CurrentPreset;

        public WindowPresets(MainWindow parent)
        {
            this.parent = parent;
            //DataContext = parent;
            InitializeComponent();
            RSControl.HideInstanceDetails();
            UpdatePresets();
            TreeViewPresets.Loaded += TreeViewPresets_Loaded;
        }

        private void UpdatePresets()
        {
            parent.RenderPresets = parent.ReadPresets();
            TreeViewPresets.Items.Clear();
            foreach (var p in parent.RenderPresets.Items)
            {
                TreeViewPresets.Items.Add(p);
            }
        }

        private void TreeViewPresets_Loaded(object sender, RoutedEventArgs e)
        {
            if (TreeViewPresets.ItemContainerGenerator.Items.Count < 1) return;
            var container = TreeViewPresets.ItemContainerGenerator.ContainerFromIndex(0);
            if (container == null)
            {
                TreeViewPresets.UpdateLayout();
                container = TreeViewPresets.ItemContainerGenerator.ContainerFromIndex(0);
                Debug.Assert(container != null, "list.ItemContainerGenerator.ContainerFromItem(item) is null, even after UpdateLayout");
            }
            (container as TreeViewItem).IsSelected = true;
            (container as TreeViewItem).IsExpanded = true;
        }

 
        private void TreeViewPresets_SelectedItemChanged(object sender, RoutedPropertyChangedEventArgs<object> e)
        {
            var item = TreeViewPresets.SelectedItem as Preset;
            if (item == null) return;  // can happen if item was deleted and changedEvent fires at the same time
            if (item.IsFolder)
            {
                return;
            }
            CurrentPreset = item;
            RSControl.GridRenderSettings.DataContext = item.Settings;

        }

        private void ButtonLoadPreset_Click(object sender, RoutedEventArgs e)
        {
            //var item = TreeViewPresets.SelectedItem as Preset;
            if (CurrentPreset != null)
                parent.LoadFromRenderSettings(CurrentPreset.Settings);
        }

        private void ButtonDeletePreset_Click(object sender, RoutedEventArgs e)
        {
            var item = TreeViewPresets.SelectedItem as Preset;
            //var item = RSControl.DataContext as Preset;
            if (item != null)
            {
                var result = MessageBox.Show($"This will permanently delete the preset ({item.PresetFileName})", "Delete Preset", MessageBoxButton.YesNo, MessageBoxImage.Warning);
                if (result == MessageBoxResult.No)
                {
                    return;
                }
                try
                {
                    File.Delete(CurrentPreset.PresetFileName);
                    UpdatePresets();
                    //TreeViewPresets.Items.RemoveAt(0);
                    //TreeViewPresets.Items.Remove(item);
                    //TreeViewPresets.Items.Clear();
                    //parent.RenderPresets = parent.ReadPresets();
                    //TreeViewPresets.Items.Add(parent.RenderPresets);
                    //TreeViewPresets.Items.Refresh();
                }
                catch (Exception ex)
                {
                    Debug.WriteLine(ex.Message);
                    new WindowErrorWithLog()
                    {
                        Title = "Error",
                        ErrorMessage = "Error deleting preset",
                        ErrorLog = ex.Message
                    }
                        .ShowDialog();
                    //throw;
                }
                
            }
        }

    }
}
