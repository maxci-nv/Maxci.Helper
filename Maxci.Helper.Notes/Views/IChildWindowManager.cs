using Maxci.Helper.Notes.Entities;
using System.Windows;

namespace Maxci.Helper.Notes.Views
{
    /// <summary>
    /// Interface for a child window manager
    /// </summary>
    interface IChildWindowManager
    {
        /// <summary>
        /// The child window
        /// </summary>
        Window ChildWindow { get; }

        /// <summary>
        /// Close all child windows
        /// </summary>
        void CloseChildWindows();

        /// <summary>
        /// Show a view for the note
        /// </summary>
        /// <param name="note">Note</param>
        void ShowNoteView(Note note);

        /// <summary>
        /// Show a view for synchronization
        /// </summary>
        void ShowSyncView();
    }
}
