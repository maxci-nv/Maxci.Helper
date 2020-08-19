using Maxci.Helper.Notes.Models;
using Maxci.Helper.Notes.Models.Impl;
using Maxci.Helper.Notes.ViewModels;
using System;
using System.Windows;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Interaction logic for NoteView.xaml
    /// </summary>
    public partial class NoteView : Window
    {
        private bool _isMaximize;


        internal NoteView(Note note)
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

            DataContext = new NoteViewModel(note, new DbRepository());
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



        #region Refresh window location

        private void MainView_SizeChanged(object sender, SizeChangedEventArgs e)
        {
            RefreshLocation();
        }

        private void MainView_LocationChanged(object sender, EventArgs e)
        {
            RefreshLocation();
        }

        private void ButtonMaximize_Click(object sender, RoutedEventArgs e)
        {
            _isMaximize = !_isMaximize;
            btnMaximize.ToolTip = (_isMaximize) ? "Minimize" : "Maximize";

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

            if (_isMaximize)
            {
                Width = 600;
                Height = mainView.Height;
                Left = mainView.Left - Width - 15;
                Top = mainView.Top;
            }
            else
            {
                Width = 600;
                Height = 250;
                Left = mainView.Left - Width - 15;
                Top = mainView.Top + (mainView.Height - Height);
            }
        }

        #endregion


    }
}
