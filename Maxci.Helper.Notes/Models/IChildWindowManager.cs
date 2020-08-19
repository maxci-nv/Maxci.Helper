using System.Windows;

namespace Maxci.Helper.Notes.Models
{
    /// <summary>
    /// Interface for managing child windows
    /// </summary>
    interface IChildWindowManager
    {
        /// <summary>
        /// The child window
        /// </summary>
        Window ChildWindow { get; }

        /// <summary>
        /// Show a view for the note
        /// </summary>
        /// <param name="note">Note</param>
        void ShowNoteView(Note note);

        /// <summary>
        /// Show a view for synchronization
        /// </summary>
        void ShowSyncView();

        /// <summary>
        /// Close all child windows
        /// </summary>
        void CloseChildWindows();
    }
}
