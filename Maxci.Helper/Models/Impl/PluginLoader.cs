using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Resources.Extensions;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace Maxci.Helper.Models.Impl
{
    /// <summary>
    /// Loader for plugins
    /// </summary>
    class PluginLoader : IPluginLoader
    {
        public IEnumerable<Plugin> GetPlugins()
        {
            var plugins = new List<Plugin>();

            if (Directory.Exists("Plugins"))
            {
                foreach (var directory in Directory.GetDirectories("Plugins"))
                {
                    var pluginName = Path.GetFileName(directory);
                    var path = $"Plugins\\{pluginName}\\{pluginName}.dll";

                    if (File.Exists(path))
                    {
                        var assembly = Assembly.LoadFrom(path);
                        var plugin = GetPlugin(assembly);

                        if (plugin?.Page != null)
                            plugins.Add(plugin);
                    }
                }
            }

            return plugins;
        }

        private static Plugin GetPlugin(Assembly assembly)
        {
            try
            {
                var ns = assembly.GetName().Name;
                var type = assembly.ExportedTypes.FirstOrDefault(t => t.FullName == $"{ns}.Views.MainView");

                if (type == null)
                    return null;

                var plugin = new Plugin
                {
                    Page = Activator.CreateInstance(type) as Page
                };

                
                using var stream = assembly.GetManifestResourceStream($"{ns}.Resources.resources");
                using var reader = new DeserializingResourceReader(stream);

                foreach (DictionaryEntry item in reader)
                {
                    switch (item.Key.ToString().ToLower())
                    {
                        case "name": plugin.Name = item.Value.ToString(); break;
                        case "caption": plugin.Caption = item.Value.ToString().ToUpper(); break;
                        case "icon": plugin.Icon = BitmapToImageSource(item.Value as byte[]); break;
                    }
                }

                return plugin;
            }
            catch
            {
                return null;
            }
        }

        private static BitmapImage BitmapToImageSource(byte[] bytes)
        {
            if (bytes == null)
                return null;

            using MemoryStream memory = new MemoryStream(bytes) { Position = 0 };

            var bitmapimage = new BitmapImage();
            bitmapimage.BeginInit();
            bitmapimage.StreamSource = memory;
            bitmapimage.CacheOption = BitmapCacheOption.OnLoad;
            bitmapimage.EndInit();

            return bitmapimage;
        }

    }
}
