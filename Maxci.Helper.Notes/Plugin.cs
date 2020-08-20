using Maxci.Helper.Notes.Models;
using Maxci.Helper.Notes.Models.Impl;
using Maxci.Helper.Notes.ViewModels;
using Maxci.Helper.Notes.Views;
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

        public static Page Create()
        {
            var view = new MainView();

            WinManager = new ChildWindowManager();
            MainViewModel = (MainViewModel)view.DataContext;
            
            return view;
        }
    }
}
