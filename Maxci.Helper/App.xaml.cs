using Maxci.Helper.Views;
using System;
using System.Windows;

namespace Maxci.Helper
{
    /// <summary>
    /// Interaction logic for App.xaml
    /// </summary>
    public partial class App : Application
    {
        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);


            if (MainWindow is MainView window)
            {
                window.Show();
                window.WindowState = WindowState.Normal;

                if (!window.IsPinned)
                    window.RefreshLocationScreen();
            }
        }

        protected override void OnDeactivated(EventArgs e)
        {
            base.OnDeactivated(e);

            if (MainWindow is MainView window && !window.IsPinned)
            {
                window.WindowState = WindowState.Minimized;
                window.Hide();
            }
        }

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            FindResource("TrayIcon");
            OnActivated(e);
        }

        private void MenuExit_Click(object sender, RoutedEventArgs e)
        {
            Shutdown();
        }

        private void TaskbarIcon_TrayLeftMouseClick(object sender, RoutedEventArgs e)
        {
            OnActivated(e);
        }
    }
}
