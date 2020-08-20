using Maxci.Helper.Notes.Models.Impl;
using Maxci.Helper.Notes.ViewModels;
using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Page
    {
        private readonly MainViewModel _context;

        public MainView()
        {
            InitializeComponent();

            _context = new MainViewModel(new DbRepository());

            DataContext = _context;

            IsVisibleChanged += MainView_IsVisibleChanged;
            Loaded += MainView_Loaded;
            Unloaded += MainView_Unloaded;
        }


        #region Refresh location of the popup panel

        private void MainView_Unloaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow != null)
                mainWindow.LocationChanged -= MainWindow_LocationChanged;
        }
        private void MainView_Loaded(object sender, RoutedEventArgs e)
        {
            var mainWindow = Application.Current.MainWindow;

            if (mainWindow != null)
                mainWindow.LocationChanged += MainWindow_LocationChanged;
        }
        private void MainWindow_LocationChanged(object sender, EventArgs e)
        {
            RefreshLocationPopup(pNewGroup);
            RefreshLocationPopup(pQuestionRemoveGroup);
            RefreshLocationPopup(pQuestionRemoveNote);
        }
        private static void RefreshLocationPopup(Popup popup)
        {
            if (popup.IsOpen)
            {
                var offset = popup.HorizontalOffset;
                popup.HorizontalOffset = offset + 1;
                popup.HorizontalOffset = offset;
            }
        }

        #endregion


        #region Refresh visibility of the child window

        private void MainView_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            var window = Plugin.WinManager?.ChildWindow;

            if (window == null)
                return;

            if (Convert.ToBoolean(e.NewValue))
                window.Show();
            else
                window.Hide();
        }

        #endregion


        #region Popups

        private void BtnShowNewGroup_Click(object sender, RoutedEventArgs e)
        {
            txtNewGroup.Text = "";
            ShowPopup(pNewGroup);
            txtNewGroup.Focus();
        }

        private void BtnShowRemoveGroup_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup(pQuestionRemoveGroup);
        }

        private void CmRemoveNote_Click(object sender, RoutedEventArgs e)
        {
            ShowPopup(pQuestionRemoveNote);
        }

        private void BtnHidePopup_Click(object sender, RoutedEventArgs e)
        {
            HidePopups();
        }

        /// <summary>
        /// Show the popup panel
        /// </summary>
        /// <param name="popup">Popup panel</param>
        private void ShowPopup(Popup popup)
        {
            HidePopups();
            popup.IsOpen = true;
        }

        /// <summary>
        /// Hide all popup panels
        /// </summary>
        private void HidePopups()
        {
            pNewGroup.IsOpen = false;
            pQuestionRemoveNote.IsOpen = false;
            pQuestionRemoveGroup.IsOpen = false;
        }

        #endregion

    }
}
