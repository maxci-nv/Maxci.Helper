using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maxci.Helper.Notes.Models
{
    /// <summary>
    /// Entity "Note" 
    /// </summary>
    class Note : INotifyPropertyChanged
    {
        /// <summary>
        /// Note ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Note group ID
        /// </summary>
        private long _idGroup;
        public long IdGroup
        {
            get => _idGroup;
            set
            {
                if (_idGroup != value)
                {
                    _idGroup = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Note name
        /// </summary>
        private string _name;
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Note text
        /// </summary>
        private string _text;
        public string Text 
        {
            get => _text;
            set
            {
                if (_text != value)
                {
                    _text = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// When the note is changed
        /// </summary>
        private DateTime _changed;
        public DateTime Changed
        {
            get => _changed;
            set
            {
                if (_changed != value)
                {
                    _changed = value;
                    OnPropertyChanged();
                }
            }
        }


        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

    }
}
