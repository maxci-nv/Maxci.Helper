using System.Windows.Controls;
using System.Windows.Media;

namespace Maxci.Helper.Models
{
    /// <summary>
    /// Entity "Plugin"
    /// </summary>
    class Plugin
    {
        /// <summary>
        /// Plugin name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Plugin caption
        /// </summary>
        public string Caption { get; set; }

        /// <summary>
        /// Plugin home page
        /// </summary>
        public Page Page { get; set; }

        /// <summary>
        /// Plugin icon
        /// </summary>
        public ImageSource Icon { get; set; }
    }
}
