﻿using Maxci.Helper.Notes.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.IO.Enumeration;
using System.Reflection;
using System.Text.Json;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for SyncView.xaml
    /// </summary>
    class SyncViewModel : BaseViewModel
    {
        private string _serverUrl;

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
            get => _serverUrl;
            set
            {
                if (_serverUrl != value)
                {
                    _serverUrl = value;
                    OnPropertyChanged();
                }
            }
        }

        public SyncViewModel()
        {
            Groups = new ObservableCollection<NoteGroup>();
            Notes = new ObservableCollection<Note>();

            _serverUrl = Plugin.Config?["AppSettings:ServerURL"];

            if (string.IsNullOrWhiteSpace(_serverUrl))
                _serverUrl = "http:\\\\127.0.0.1";

        }



        #region Commands

        private ICommand _saveConfigCommand;
        public ICommand SaveConfigCommand => _saveConfigCommand ?? (_saveConfigCommand = new RelayCommand(obj =>
        {
            AddOrUpdateConfig("AppSettings:ServerURL", _serverUrl);
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

                    if (jsonObj[keys[keys.Length - 1]] == null)
                        jsonObj[keys[keys.Length - 1]] = new JValue(value);
                    else
                        ((JValue)jsonObj[keys[keys.Length - 1]]).Value = value;

                    var output = JsonConvert.SerializeObject(jsonDocument, Formatting.Indented);
                    File.WriteAllText(filename, output);

                    Plugin.Config[key] = _serverUrl;
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
