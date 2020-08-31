using Maxci.Helper.Notes.Entities;
using System;
using System.Collections.Generic;

namespace Maxci.Helper.Notes.Repositories
{
    /// <summary>
    /// Interface for database
    /// </summary>
    interface IDbRepository
    {
        /// <summary>
        /// Returns a list of note groups
        /// </summary>
        /// <returns>List of note groups</returns>
        IEnumerable<NoteGroup> GetGroups();

        /// <summary>
        /// Returns a list of notes
        /// </summary>
        /// <param name="idGroup">ID group</param>
        /// <returns>List of notes</returns>
        IEnumerable<Note> GetNotes(long idGroup);

        /// <summary>
        /// Add a new group of notes
        /// </summary>
        /// <param name="guid">Global ID for the group</param>
        /// <param name="name">Group name</param>
        /// <returns>ID group</returns>
        NoteGroup AddGroup(Guid guid, string name);

        /// <summary>
        /// Remove the group of notes
        /// </summary>
        /// <param name="id">ID group</param>
        /// <returns>True if the group has been removed</returns>
        bool RemoveGroup(long id);

        /// <summary>
        /// Add a new note
        /// </summary>
        /// <param name="guid">Global ID for the note</param>
        /// <param name="idGroup">ID group</param>
        /// <param name="name">Note name</param>
        /// <param name="text">Note text</param>
        /// <returns>ID note</returns>
        Note AddNote(Guid guid, long idGroup, string name, string text);

        /// <summary>
        /// Remove the note
        /// </summary>
        /// <param name="id">ID note</param>
        /// <returns>True if the note has been removed</returns>
        bool RemoveNote(long id);

        /// <summary>
        /// Update the note
        /// </summary>
        /// <param name="id">ID note</param>
        /// <param name="idGroup">ID group</param>
        /// <param name="name">Note name</param>
        /// <param name="text">Note text</param>
        /// <returns>True if the note has been updated</returns>
        bool UpdateNote(long id, long idGroup, string name, string text);



        #region Data Sync

        IEnumerable<dynamic> GetGroupsForSync();

        /// <summary>
        /// Add a new group of notes (for data sync)
        /// </summary>
        /// <param name="guid">Global ID for the group</param>
        /// <param name="name">Group name</param>
        /// <param name="sync">GUID for sync</param>
        /// <returns>ID group</returns>
        bool AddGroupForSync(Guid guid, string name, Guid sync);

        /// <summary>
        /// Remove the group of notes (for data sync)
        /// </summary>
        /// <param name="guid">Global ID group</param>
        /// <param name="sync">GUID for sync</param>
        /// <returns>True if the group has been removed</returns>
        bool RemoveGroupForSync(Guid guid, Guid sync);

        /// <summary>
        /// Add a new note (for data sync)
        /// </summary>
        /// <param name="guid">Global ID for the note</param>
        /// <param name="idGroup">ID group</param>
        /// <param name="name">Note name</param>
        /// <param name="text">Note text</param>
        /// <param name="sync">GUID for sync</param>
        /// <returns>ID note</returns>
        bool AddNoteForSync(Guid guid, long idGroup, string name, string text, Guid sync );

        /// <summary>
        /// Update the note
        /// </summary>
        /// <param name="guid">Global ID note</param>
        /// <param name="idGroup">ID group</param>
        /// <param name="name">Note name</param>
        /// <param name="text">Note text</param>
        /// <param name="sync">GUID for sync</param>
        /// <returns>True if the note has been updated</returns>
        bool UpdateNoteForSync(Guid guid, long idGroup, string name, string text, Guid sync);

        #endregion
    }
}
