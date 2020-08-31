namespace Maxci.Helper.Notes.Entities
{
    /// <summary>
    /// Entity "Note group"
    /// </summary>
    public class NoteGroup : ObservableObject
    {
        private string _name;

        /// <summary>
        /// Note group ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// Note group name
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
    }
}
