using Grace.DependencyInjection;
using Grace.DependencyInjection.Attributes;
using Maxci.Helper.Notes.Repositories;
using Maxci.Helper.Notes.ViewModels;
using Maxci.Helper.Notes.Views;
using Microsoft.Extensions.Configuration;
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
        private static DependencyInjectionContainer _container;
        private static MainViewModel _mainViewModel;

        public static Page Create()
        {
            var path = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            var configuration = new ConfigurationBuilder()
                .SetBasePath(path)
                .AddJsonFile("appsettings.json", true)
                .Build();

            _container = GetContainer(configuration);

            var view = _container.Locate<MainView>();
            view.DataContext = _container.Locate<MainViewModel>();

            return view;
        }

        private static DependencyInjectionContainer GetContainer(IConfiguration configuration)
        {
            var container = new DependencyInjectionContainer();

            container.Configure(z =>
            {
                z.ExportInstance(configuration);
                z.ExportInstance(new ChildWindowManager());
                z.ExportAs<DbRepository, IDbRepository>();
                z.ExportAs<ServerRepository, IServerRepository>();

                z.ExportFactory<IExportLocatorScope, MainViewModel>(scope =>
                {
                    if (_mainViewModel == null)
                        _mainViewModel = new MainViewModel(scope.Locate<IDbRepository>())
                        {
                            WinManager = scope.Locate<ChildWindowManager>()
                        };

                    return _mainViewModel;
                });

                z.ImportMembers<object>(MembersThat.HaveAttribute<ImportAttribute>());
            });

            return container;
        }
    }
}
