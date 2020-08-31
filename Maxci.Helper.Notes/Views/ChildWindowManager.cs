using Maxci.Helper.Notes.Entities;
using Maxci.Helper.Notes.Repositories;
using Maxci.Helper.Notes.ViewModels;
using System.Windows;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Child window manager
    /// </summary>

    public class ChildWindowManager: IChildWindowManager
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
                    ChildWindow.DataContext = new NoteViewModel(note, new DbRepository(), null);

                ChildWindow.Show();
            }
        }

        public void ShowSyncView()
        {
            var childWindow = ChildWindow;

            if (!(ChildWindow is SyncView))
            {
                CloseChildWindows();

                ChildWindow = new SyncView(null);
                ChildWindow.Show();
            }
            else
                childWindow.Show();
        }

    }
}
