using System;

namespace Maxci.Helper.Notes.Entities
{
    /// <summary>
    /// Entity "Note" 
    /// </summary>
    public class Note : ObservableObject
    {
        private long _idGroup;
        private string _name;
        private string _text;
        private DateTime _changed;

        /// <summary>
        /// Note ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Note group ID
        /// </summary>
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

    }
}
