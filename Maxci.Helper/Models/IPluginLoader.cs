using System.Collections.Generic;

namespace Maxci.Helper.Models
{
    /// <summary>
    /// Loader for plugins
    /// </summary>
    interface IPluginLoader
    {
        /// <summary>
        /// Return list of plugins
        /// </summary>
        /// <returns>List of plugins</returns>
        IEnumerable<Plugin> GetPlugins();
    }
}
