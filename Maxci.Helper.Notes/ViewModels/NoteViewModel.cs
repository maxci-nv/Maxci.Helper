using Maxci.Helper.Notes.Models;
using System;
using System.Windows;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for NoteView.xaml
    /// </summary>
    class NoteViewModel : BaseViewModel
    {
        private readonly IDbRepository _db;
        private readonly Note _note_old;
        private string _noteName;
        private string _noteText;
        private DateTime _noteChanged;


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
        public DateTime NoteChanged
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


        public NoteViewModel(Note note, IDbRepository db)
        {
            _note_old = note ?? throw new ArgumentNullException(nameof(note));
            _db = db ?? throw new ArgumentNullException(nameof(db));

            if (note != null)
            {
                NoteName = note.Name;
                NoteText = note.Text;
                NoteChanged = note.Changed;
            }
        }



        #region Commands

        private ICommand _saveCommand;
        public ICommand SaveCommand => _saveCommand ?? (_saveCommand = new RelayCommand(obj =>
        {
            if (string.IsNullOrWhiteSpace(_noteName))
            {
                MessageBox.Show("Enter a title for your note!");
                return;
            }

            if (_db.UpdateNote(_note_old.Id, _note_old.IdGroup, _noteName, _noteText))
            {
                _note_old.Name = _noteName;
                _note_old.Text = _noteText;
                _note_old.Changed = DateTime.UtcNow;

                NoteChanged = _note_old.Changed;
            }
        }, obj => !string.IsNullOrWhiteSpace(_noteName)));

        #endregion
    }
}
