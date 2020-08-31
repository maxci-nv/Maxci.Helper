using System.Collections.Generic;

namespace Maxci.Helper.Notes.Repositories
{
    /// <summary>
    /// Interface for server
    /// </summary>
    interface IServerRepository
    {
        /// <summary>
        /// Server URL
        /// </summary>
        string ServerURL { get; set; }

        /// <summary>
        /// Returns list of note groups
        /// </summary>
        /// <returns>List of note groups</returns>
        IEnumerable<dynamic> GetGroups();
    }
}
