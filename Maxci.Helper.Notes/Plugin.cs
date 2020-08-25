using Maxci.Helper.Notes.Models;
using Maxci.Helper.Notes.Models.Impl;
using Maxci.Helper.Notes.ViewModels;
using Maxci.Helper.Notes.Views;
using Microsoft.Extensions.Configuration;
using System;
using System.IO;
using System.Reflection;
using System.Windows.Controls;

namespace Maxci.Helper.Notes
{
    /// <summary>
    /// Plugin entry point
    /// </summary>
    public static class Plugin
    {
        internal static MainViewModel MainViewModel;
        internal static IChildWindowManager WinManager;
        internal static IConfiguration Config;
        public static Page Create()
        {
            var view = new MainView();
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            WinManager = new ChildWindowManager();
            MainViewModel = (MainViewModel)view.DataContext;
            Config = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", true)
                .Build();

            return view;
        }
    }
}
