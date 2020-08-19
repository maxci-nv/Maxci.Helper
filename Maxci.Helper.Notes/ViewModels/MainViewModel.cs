using Maxci.Helper.Notes.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Threading;
using System.Windows;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for MainView.xaml
    /// </summary>
    class MainViewModel : BaseViewModel
    {
        private readonly IChildWindowManager _winManager;
        private readonly IDbRepository _db;
        private NoteGroup _currentGroup;
        private Note _currentNote;


        /// <summary>
        /// List of note groups
        /// </summary>
        public ObservableCollection<NoteGroup> NoteGroups { get; set; }

        /// <summary>
        /// List of notes
        /// </summary>s
        public ObservableCollection<Note> Notes { get; set; }

        /// <summary>
        /// Current note group
        /// </summary>
        public NoteGroup CurrentGroup
        {
            get => _currentGroup;
            set
            {
                if (_currentGroup != value)
                {
                    _currentGroup = value;
                    OnPropertyChanged();

                    RefreshNotes();
                    CurrentNote = null;
                }
            }
        }

        /// <summary>
        /// Current note
        /// </summary>
        public Note CurrentNote
        {
            get => _currentNote;
            set
            {
                if (_currentNote != value)
                {
                    _currentNote = value;
                    OnPropertyChanged();

                    _winManager.ShowNoteView(value);
                }
            }
        }



        public MainViewModel(IDbRepository db, IChildWindowManager windowManager)
        {
            _db = db ?? throw new ArgumentNullException(nameof(db));
            _winManager = windowManager ?? throw new ArgumentNullException(nameof(windowManager));

            NoteGroups = new ObservableCollection<NoteGroup>();
            Notes = new ObservableCollection<Note>();

            LoadGroups();

            if (NoteGroups.Count > 0)
                CurrentGroup = NoteGroups[0];
        }



        #region Commands

        /// <summary>
        /// Add a new group of notes
        /// </summary>
        private ICommand _addGroupCommand;
        public ICommand AddGroupCommand => _addGroupCommand ?? (_addGroupCommand = new RelayCommand(obj =>
        {
            var nameGroup = obj?.ToString().Trim();
            var guidGroup = Guid.NewGuid();
            var group = _db.AddGroup(guidGroup, nameGroup);

            if (group != null)
            {
                var pos = GetPositionForInsert(group);
                NoteGroups.Insert(pos, group);
                CurrentGroup = group;
            }
        }, obj => !string.IsNullOrWhiteSpace(obj?.ToString()) ));
        /// <summary>
        /// Returns position index to insert note group
        /// </summary>
        /// <param name="group">Note group</param>
        /// <returns>Position index</returns>
        private int GetPositionForInsert(NoteGroup group)
        {
            var groups = NoteGroups;
            var count = groups.Count;

            if (count == 0)
                return 0;

            var groupName = group.Name;
            var start = 0;
            var finish = count - 1;

            count = finish - start + 1;

            do
            {
                if (count == 1)
                    return (groupName.CompareTo(groups[start].Name) >= 0) ? start + 1 : start;

                if (count == 2)
                {
                    var result = groupName.CompareTo(groups[finish].Name);

                    if (result >= 0)
                        return finish + 1;

                    result = groupName.CompareTo(groups[start].Name);

                    if (result < 0)
                        return start;

                    return finish;
                }

                var i = (count / 2) + start;
                var resultCompare = groupName.CompareTo(groups[i].Name);

                if (resultCompare > 0)
                    start = i;
                else if (resultCompare < 0)
                    finish = i;
                else
                    start = finish + 1;

                count = finish - start + 1;
            }
            while (true);
        }


        /// <summary>
        /// Remove the group of notes
        /// </summary>
        private ICommand _removeGroupCommand;
        public ICommand RemoveGroupCommand => _removeGroupCommand ?? (_removeGroupCommand = new RelayCommand(obj =>
        {
            if (_currentGroup == null || _currentGroup.Id == 0)
                return;

            if (_db.RemoveGroup(_currentGroup.Id))
            {
                var groups = NoteGroups;

                if (groups.Contains(_currentGroup))
                    groups.Remove(_currentGroup);
    
                CurrentGroup = (groups.Count > 0) ? groups[0] : null;
            }
        }, obj => _currentGroup != null));


        /// <summary>
        /// Add a new note
        /// </summary>
        private ICommand _addNoteCommand;
        public ICommand AddNoteCommand => _addNoteCommand ?? (_addNoteCommand = new RelayCommand(obj =>
        {
            var idGroup = _currentGroup?.Id ?? 0;

            if (idGroup <= 0)
                return;

            var nameNote = "New note";
            var textNote = "";
            var guidNote = Guid.NewGuid();
            var note = _db.AddNote(guidNote, idGroup, nameNote, textNote);

            if (note != null)
            {
                Notes.Add(note);
                CurrentNote = note;
            }
        }, obj => _currentGroup != null));


        /// <summary>
        /// Remove the note
        /// </summary>
        private ICommand _removeNoteCommand;
        public ICommand RemoveNoteCommand => _removeNoteCommand ?? (_removeNoteCommand = new RelayCommand(obj =>
        {
            if (_currentNote == null)
                return;

            if (_db.RemoveNote(_currentNote.Id))
            {
                Notes.Remove(_currentNote);
                CurrentNote = null;
            }
        }, obj => _currentNote != null));


        /// <summary>
        /// Sync notes with the server
        /// </summary>
        private ICommand _syncNotesCommand;
        public ICommand SyncNotesCommand => _syncNotesCommand ?? (_syncNotesCommand = new RelayCommand(obj =>
        {
            MessageBox.Show("NotImplemented");
            
            //CurrentNote = null;
            //_winManager.ShowSyncView();
        }));
        
        #endregion



        /// <summary>
        /// Load a list of note groups
        /// </summary>
        private void LoadGroups()
        {
            var groups = NoteGroups;
            groups.Clear();

            foreach (var group in _db.GetGroups())
                groups.Add(group);
        }

        /// <summary>
        /// Load a list of notes
        /// </summary>
        private void RefreshNotes()
        {
            var notes = Notes;
            notes.Clear();

            if (CurrentGroup != null)
                foreach (var note in _db.GetNotes(CurrentGroup.Id))
                    notes.Add(note);
        }
    }
}
