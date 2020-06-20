using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace DerOptimist
{
    public static class Commands
    {
        public static readonly RoutedUICommand Exit = new RoutedUICommand
            (
                "Exit",
                "Exit",
                typeof(Commands),
                new InputGestureCollection()
                {
                    new KeyGesture(Key.F4, ModifierKeys.Alt)
                }
            );

        public static readonly RoutedUICommand CopyToClipboard = new RoutedUICommand
            (
                "Copy to Clipboard",
                "CopyToClipboard",
                typeof(Commands)
            );

        public static readonly RoutedUICommand OpenRecent = new RoutedUICommand
            (
                "Open Recent",
                "OpenRecent",
                typeof(Commands)
            );
    }
}
