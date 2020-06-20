using System;
using System.Collections.Generic;
using System.Diagnostics;
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

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for RenderSettingsControl.xaml
    /// </summary>
    public partial class RenderSettingsControl : UserControl
    {
        public WindowRenderQueue parentQueue { get; set; } = null;

        public RenderSettingsControl()
        {
            InitializeComponent();
        }

        public void HideInstanceDetails()
        {
            GridRenderInstanceDetails.Visibility = Visibility.Collapsed;
        }
        public void ShowInstanceDetails()
        {
            GridRenderInstanceDetails.Visibility = Visibility.Visible;
        }

        private void ButtonTagToClipboard_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Button))
                {
                    Clipboard.SetText((sender as Button).Tag.ToString());
                }
                else if (sender.GetType() == typeof(Hyperlink))
                {
                    Clipboard.SetText((sender as Hyperlink).Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ButtonTagOpen_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (sender.GetType() == typeof(Button))
                {
                    Process.Start((sender as Button).Tag.ToString());
                }
                else if (sender.GetType() == typeof(Hyperlink))
                {
                    Process.Start((sender as Hyperlink).Tag.ToString());
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ButtonExplore_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                Process.Start("explorer.exe", $"/select, \"{(sender as Button).Tag.ToString()}\"");
                //Process.Start("explorer.exe", "/select " + (sender as Button).Tag.ToString());
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }

        private void ButtonTagChangeOutput_Click(object sender, RoutedEventArgs e)
        {
            try
            {
                if (parentQueue != null)
                {
                    parentQueue.MenuItem_ChangeOutputPath_Click(sender, e);
                }
            }
            catch (Exception ex)
            {
                Debug.WriteLine(ex.Message);
            }
        }
    }
}
