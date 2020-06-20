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
using System.Windows.Shapes;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for WindowErrorWithLog.xaml
    /// </summary>
    public partial class WindowErrorWithLog : Window
    {
        public string ErrorMessage
        {
            get { return (string)GetValue(ErrorMessageProperty); }
            set { SetValue(ErrorMessageProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.
        public static readonly DependencyProperty ErrorMessageProperty =
            DependencyProperty.Register("ErrorMessage", typeof(string), typeof(WindowErrorWithLog), new PropertyMetadata(default(string)));

        public string ErrorLog
        {
            get { return (string)GetValue(ErrorLogProperty); }
            set { SetValue(ErrorLogProperty, value); }
        }

        // Using a DependencyProperty as the backing store for ErrorMessage.
        public static readonly DependencyProperty ErrorLogProperty =
            DependencyProperty.Register("ErrorLog", typeof(string), typeof(WindowErrorWithLog), new PropertyMetadata(default(string)));

        public WindowErrorWithLog()
        {
            InitializeComponent();
        }

        private void ButtonOK_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
    }
}
