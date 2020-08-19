using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// Common class for ViewModels
    /// </summary>
    abstract class BaseViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
