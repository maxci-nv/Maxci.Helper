using Maxci.Helper.Notes.Entities;
using Maxci.Helper.Notes.Repositories;
using Maxci.Helper.Notes.ViewModels;
using System.Windows;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Child windows manager
    /// </summary>

    public class ChildWindowManager
    {
        /// <summary>
        /// The child window
        /// </summary>
        public Window ChildWindow { get; private set; }

        /// <summary>
        /// Close all child windows
        /// </summary>
        public void CloseChildWindows()
        {
            if (ChildWindow != null)
            {
                ChildWindow.Close();
                ChildWindow = null;
            }
        }

        /// <summary>
        /// Show a view for the note
        /// </summary>
        /// <param name="note">Note</param>
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

        /// <summary>
        /// Show a view for synchronization
        /// </summary>
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
