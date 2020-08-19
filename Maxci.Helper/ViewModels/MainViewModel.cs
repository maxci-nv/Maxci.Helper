using Maxci.Helper.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;

namespace Maxci.Helper.ViewModels
{
    /// <summary>
    /// ViewModel for MainView.xaml
    /// </summary>
    class MainViewModel : INotifyPropertyChanged
    {
        const string CAPTION = "Maxci";

        private string _caption;
        private Plugin _activePlugin;


        /// <summary>
        /// List of plugins
        /// </summary>
        public ObservableCollection<Plugin> Plugins { get; set; }

        /// <summary>
        /// Window caption
        /// </summary>
        public string Caption
        {
            get => _caption;
            set
            {
                if (_caption != value)
                {
                    _caption = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Active plugin
        /// </summary>
        public Plugin ActivePlugin
        {
            get => _activePlugin;
            set
            {
                if (_activePlugin != value)
                {
                    _activePlugin = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(VisiblePlugin));
                }
            }
        }

        /// <summary>
        /// Flag "Plugin is visibled"
        /// </summary>
        public bool VisiblePlugin
        {
            get => _activePlugin != null;
        }


        public MainViewModel(IPluginLoader pluginLoader)
        {
            Plugins = new ObservableCollection<Plugin>();
            Caption = CAPTION;
            ActivePlugin = null;

            var plugins = pluginLoader?.GetPlugins();

            if (plugins != null)
            {
                foreach (var plugin in plugins)
                    Plugins.Add(plugin);
            }
        }



        #region Commands

        /// <summary>
        /// Open the plugin
        /// </summary>
        private ICommand _pluginOpenCommand;
        public ICommand PluginOpenCommand => _pluginOpenCommand ?? (_pluginOpenCommand = new RelayCommand(obj =>
        {
            if (obj is Plugin plugin)
            {
                plugin.Page.Visibility = Visibility.Visible;
                Caption = plugin.Caption;
                ActivePlugin = plugin;
            }
        }));

        /// <summary>
        /// Close the plugin
        /// </summary>
        private ICommand _pluginCloseCommand;
        public ICommand PluginCloseCommand => _pluginCloseCommand ?? (_pluginCloseCommand = new RelayCommand(obj =>
        {
            Caption = CAPTION;

            if (ActivePlugin != null)
            {
                ActivePlugin.Page.Visibility = Visibility.Hidden;
                ActivePlugin = null;
            }
        }));

        #endregion



        #region interface INotifyPropertyChanged

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Occurs when a property value changes
        /// </summary>
        private void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        #endregion

    }
}
