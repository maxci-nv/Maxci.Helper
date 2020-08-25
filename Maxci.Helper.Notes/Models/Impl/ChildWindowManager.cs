using Maxci.Helper.Notes.ViewModels;
using Maxci.Helper.Notes.Views;
using System.Windows;

namespace Maxci.Helper.Notes.Models.Impl
{
    class ChildWindowManager : IChildWindowManager
    {
        public Window ChildWindow { get; private set; }
        
        public void CloseChildWindows()
        {
            if (ChildWindow != null)
            {
                ChildWindow.Close();
                ChildWindow = null;
            }
        }

        public void ShowNoteView(Note note)
        {
            if (note == null)
            {
                if (ChildWindow is NoteView)
                    CloseChildWindows();
            }
            else
            {
                if (!(ChildWindow is NoteView))
                    CloseChildWindows();

                if (ChildWindow == null)
                    ChildWindow = new NoteView(note);
                else if (ChildWindow is NoteView)
                    ChildWindow.DataContext = new NoteViewModel(note, new DbRepository());

                ChildWindow.Show();
            }
        }

        public void ShowSyncView()
        {
            var childWindow = ChildWindow;

            if (!(ChildWindow is SyncView))
            {
                CloseChildWindows();

                ChildWindow = new SyncView();
                ChildWindow.Show();
            }
            else
                childWindow.Show();
        }
    }
}
