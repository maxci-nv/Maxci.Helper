using Grace.DependencyInjection.Attributes;
using Maxci.Helper.Notes.Repositories;
using Maxci.Helper.Notes.ViewModels;
using Microsoft.Extensions.Configuration;
using System;
using System.Windows;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Interaction logic for SyncView.xaml
    /// </summary>
    public partial class SyncView : Window
    {
        [Import]
        public ChildWindowManager WinManager { get; set; }

        public SyncView(IConfiguration configuration)
        {
            InitializeComponent();
            RefreshLocation();

            var mainView = Application.Current?.MainWindow;

            if (mainView != null)
            {
                mainView.LocationChanged += MainView_LocationChanged;
                mainView.SizeChanged += MainView_SizeChanged;
                mainView.StateChanged += MainView_LocationChanged;
            }

            DataContext = new SyncViewModel(configuration, new ServerRepository(), new DbRepository());
        }

        protected override void OnClosed(EventArgs e)
        {
            var mainView = Application.Current?.MainWindow;

            if (mainView != null)
            {
                mainView.LocationChanged -= MainView_LocationChanged;
                mainView.SizeChanged -= MainView_SizeChanged;
                mainView.StateChanged -= MainView_LocationChanged;
            }
        }

        private void BtnClose_Click(object sender, RoutedEventArgs e)
        {
            WinManager?.CloseChildWindows();
        }



        #region Refresh window location

        private void MainView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshLocation();
        }

        private void MainView_LocationChanged(object sender, EventArgs e)
        {
            RefreshLocation();
        }

        /// <summary>
        /// Refresh window location
        /// </summary>
        private void RefreshLocation()
        {
            var mainView = Application.Current?.MainWindow;

            if (mainView == null)
                return;

            Width = 600;
            Height = mainView.Height;
            Left = mainView.Left - Width - 15;
            Top = mainView.Top;
        }

        #endregion
    }
}
