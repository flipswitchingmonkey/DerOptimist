using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;

namespace DerOptimist
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        private void Application_Startup(object sender, StartupEventArgs e)
        {
            MainWindow window = new MainWindow();
            if (e.Args.Length > 0)
            {
                string fn = e.Args[0];
                if (File.Exists(fn))
                {
                    window.OpenFile(fn);
                }
            }
            window.Show();
        }
    }
}
