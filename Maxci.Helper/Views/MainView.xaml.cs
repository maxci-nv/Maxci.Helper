using Maxci.Helper.Models.Impl;
using System.Windows;

namespace Maxci.Helper.Views
{
    /// <summary>
    /// Interaction logic for MainView.xaml
    /// </summary>
    public partial class MainView : Window
    {
        const int WIDTH = 250;
        const int TOP = 20;

        public MainView()
        {
            InitializeComponent();

            var loader = new PluginLoader();
            var context = new ViewModels.MainViewModel(loader);
            DataContext = context;
        }

        /// <summary>
        /// The window is pinned
        /// </summary>
        public bool IsPinned { get => (btnPushpin?.IsChecked == true) ? true : false; }

        /// <summary>
        /// Refresh positions and size of window on the screen
        /// </summary>
        public void RefreshLocationScreen()
        {
            Width = WIDTH;
            Height = SystemParameters.PrimaryScreenHeight - TOP - 50;
            Top = TOP;
            Left = SystemParameters.PrimaryScreenWidth - WIDTH;
        }

        /// <summary>
        /// Event handler "Mouse left button pressed on the header"
        /// <para>Used to drag the window</para>
        /// </summary>
        private void TextBlock_MouseLeftButtonDown(object sender, System.Windows.Input.MouseButtonEventArgs e)
        {
            DragMove();
        }
    }
}
