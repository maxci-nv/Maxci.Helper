using Maxci.Helper.Notes.Models;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Maxci.Helper.Notes.ViewModels
{
    /// <summary>
    /// ViewModel for SyncView.xaml
    /// </summary>
    class SyncViewModel : BaseViewModel
    {
        public ObservableCollection<NoteGroup> Groups;
        public ObservableCollection<Note> Notes;


        public SyncViewModel()
        {
            Groups = new ObservableCollection<NoteGroup>();
        }



        #region Commands

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
