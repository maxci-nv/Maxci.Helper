using Maxci.Helper.Notes.Entities;
using Maxci.Helper.Notes.Repositories;
using System;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for NoteView.xaml
    /// </summary>
    class NoteViewModel : ObservableObject
    {
        private readonly MainViewModel _mainViewModel;
        private readonly IDbRepository _db;
        private readonly Note _note;
        private string _noteName;
        private string _noteText;
        private DateTime? _noteChanged;

        /// <summary>
        /// Is new note?
        /// </summary>
        public bool IsNew { get; private set; }

        /// <summary>
        /// Note name
        /// </summary>
        public string NoteName 
        {
            get => _noteName;
            set
            {
                if (_noteName != value)
                {
                    _noteName = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Note text
        /// </summary>
        public string NoteText
        {
            get => _noteText;
            set
            {
                if (_noteText != value)
                {
                    _noteText = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Time when the note was changed
        /// </summary>
        public DateTime? NoteChanged
        {
            get => _noteChanged;
            set
            {
                if (_noteChanged != value)
                {
                    _noteChanged = value;
                    OnPropertyChanged();
                }
            }
        }


        public NoteViewModel(Note note, IDbRepository db, MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel ?? throw new ArgumentNullException(nameof(mainViewModel));
            _note = note ?? throw new ArgumentNullException(nameof(note));
            _db = db ?? throw new ArgumentNullException(nameof(db));

            if (note != null)
            {
                IsNew = (note.Id <= 0);

                NoteName = note.Name;
                NoteText = note.Text;
                NoteChanged = IsNew ? (DateTime?)null : note.Changed;
            }
        }



        #region Commands

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(obj =>
        {
            if (IsNew)
            {
                var note = _db.AddNote(Guid.NewGuid(), _note.IdGroup, _noteName, _noteText);

                if (note != null)
                {
                    _note.Id = note.Id;
                    _note.Name = _noteName;
                    _note.Text = _noteText;
                    _note.Changed = DateTime.UtcNow;

                    NoteChanged = _note.Changed;
                    IsNew = false;

                    var pos = GetPositionForInsert(note);
                    _mainViewModel.Notes.Insert(pos, _note);
                    _mainViewModel.CurrentNote = _note;
                }
            }
            else
            {
                if (_db.UpdateNote(_note.Id, _note.IdGroup, _noteName, _noteText))
                {
                    var isRename = _note.Name != _noteName;

                    if (isRename)
                    {
                        var indexNew = GetPositionForInsert(new Note { Name = _noteName });
                        var notes = _mainViewModel.Notes;
                        var indexOld = notes.IndexOf(_note);

                        if (indexNew > indexOld)
                            indexNew--;

                        _note.Name = _noteName;
                        _note.Text = _noteText;

                        notes.Move(indexOld, indexNew);
                    }
                    else
                    {
                        _note.Name = _noteName;
                        _note.Text = _noteText;
                    }

                    _note.Changed = DateTime.UtcNow;
                    NoteChanged = _note.Changed;
                }
            }


        }, obj => !string.IsNullOrWhiteSpace(_noteName)));
        /// <summary>
        /// Returns position index to insert note
        /// </summary>
        /// <param name="note">Note</param>
        /// <returns>Position index</returns>
        private int GetPositionForInsert(Note note)
        {
            var notes = _mainViewModel.Notes;
            var count = notes.Count;

            if (count == 0)
                return 0;

            var noteName = note.Name;
            var start = 0;
            var finish = count - 1;

            count = finish - start + 1;

            do
            {
                if (count == 0)
                    return start;

                if (count == 1)
                    return (noteName.CompareTo(notes[start].Name) >= 0) ? start + 1 : start;

                if (count == 2)
                {
                    var result = noteName.CompareTo(notes[finish].Name);

                    if (result >= 0)
                        return finish + 1;

                    result = noteName.CompareTo(notes[start].Name);

                    if (result < 0)
                        return start;

                    return finish;
                }

                var i = (count / 2) + start;
                var resultCompare = noteName.CompareTo(notes[i].Name);

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

        #endregion
    }
}
