using Maxci.Helper.Notes.Entities;
using Maxci.Helper.Notes.Repositories;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System.Collections.ObjectModel;
using System.IO;
using System.Reflection;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for SyncView.xaml
    /// </summary>
    class SyncViewModel : ObservableObject
    {
        private readonly IConfiguration _configuration;
        private readonly IServerRepository _server;
        private readonly IDbRepository _db;

        /// <summary>
        /// List of note groups
        /// </summary>
        public ObservableCollection<NoteGroup> Groups { get; set; }

        /// <summary>
        /// List of notes
        /// </summary>
        public ObservableCollection<Note> Notes { get; set; }

        /// <summary>
        /// Server URL
        /// </summary>
        public string ServerURL 
        {
            get => _server.ServerURL;
            set
            {
                if (_server.ServerURL != value)
                {
                    _server.ServerURL = value;
                    OnPropertyChanged();
                }
            }
        }

        public SyncViewModel(IConfiguration configuration, IServerRepository server, IDbRepository db)
        {
            Groups = new ObservableCollection<NoteGroup>();
            Notes = new ObservableCollection<Note>();

            _configuration = configuration;
            _server = server;
            _db = db;

            if (_server != null)
            {
                var serverUrl = _configuration["AppSettings:ServerURL"];

                if (string.IsNullOrWhiteSpace(serverUrl))
                    serverUrl = "http:\\\\127.0.0.1";

                _server.ServerURL = serverUrl;
            }

            InitGroups();
        }


        private void InitGroups()
        {
            //var groups_client = _db.GetGroupsForSync();
            //var groups_server = _server.GetGroups();
            //var groups_result = new SortedSet();
            //
            //var t = groups_server.Union(groups_client);

            

        }



        #region Commands

        private ICommand _saveConfigCommand;
        public ICommand SaveConfigCommand => _saveConfigCommand ?? (_saveConfigCommand = new RelayCommand(obj =>
        {
            AddOrUpdateConfig("AppSettings:ServerURL", _server.ServerURL);
        }));
        private bool AddOrUpdateConfig<T>(string key, T value)
        {
            try
            {
                var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                var filename = Path.Combine(path, "appsettings.json");

                if (File.Exists(filename))
                {
                    var json = File.ReadAllText(filename);
                    var jsonDocument = JToken.Parse(json);
                    var jsonObj = jsonDocument;
                    var keys = key.Split(':');

                    for (var i=0; i < keys.Length-1; i++)
                    {
                        var section = keys[i];
                        var token = jsonObj[section];

                        if (token == null)
                            jsonObj[section] = new JObject();

                        jsonObj = jsonObj[section];
                    }

                    if (jsonObj[keys[^1]] == null)
                        jsonObj[keys[^1]] = new JValue(value);
                    else
                        ((JValue)jsonObj[keys[^1]]).Value = value;

                    var output = JsonConvert.SerializeObject(jsonDocument, Formatting.Indented);
                    File.WriteAllText(filename, output);

                    _configuration[key] = value?.ToString();
                }

                return true;
            }
            catch
            {
                return false;
            }
        }


        private ICommand _refreshGroupsCommand;
        public ICommand RefreshGroupsCommand => _refreshGroupsCommand ?? (_refreshGroupsCommand = new RelayCommand(obj =>
        {

        }));


        private ICommand _refreshNotesCommand;
        public ICommand RefreshNotesCommand => _refreshNotesCommand ?? (_refreshNotesCommand = new RelayCommand(obj =>
        {

        }));


        private ICommand _syncGroupsCommand;
        public ICommand SyncGroupsCommand => _syncGroupsCommand ?? (_syncGroupsCommand = new RelayCommand(obj =>
        {

        }));


        private ICommand _syncNotesCommand;
        public ICommand SyncNotesCommand => _syncNotesCommand ?? (_syncNotesCommand = new RelayCommand(obj =>
        {

        }));

        #endregion
    }
}
