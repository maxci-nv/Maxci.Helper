using Maxci.Helper.Models;
using Maxci.Helper.ViewModels;
using NSubstitute;
using NUnit.Framework;
using System.Collections;
using System.ComponentModel;
using System.Threading;
using System.Windows;
using System.Windows.Controls;

namespace Maxci.Helper.UnitTests.Helper.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Упростите инициализацию объекта", Justification = "<Ожидание>")]
    [TestFixture]
    class MainViewModelTests
    {
        [Test]
        public void Ctor_InitProperties__ActivePluginIsNull_CaptionIsNotEmpty_PluginsIsNotNull()
        {
            // Arrange
            var pluginLoader = Substitute.For<IPluginLoader>();

            // Act
            var viewModel = new MainViewModel(pluginLoader);

            // Assert
            Assert.That(viewModel.Plugins, Is.Not.Null);
            Assert.That(viewModel.ActivePlugin, Is.Null);
            Assert.That(viewModel.Caption, Is.Not.Empty);
        }

        [Test]
        public void Ctor_PluginsLoaded_PluginsIsNotEmpty()
        {
            // Arrange
            var plugin1 = Substitute.For<Plugin>();
            plugin1.Name = "1";

            var plugin2 = Substitute.For<Plugin>();
            plugin2.Name = "2";

            var pluginLoader = Substitute.For<IPluginLoader>();
            pluginLoader.GetPlugins().Returns(new[] { plugin1, plugin2 });

            // Act
            var viewModel = new MainViewModel(pluginLoader);

            // Assert
            pluginLoader.Received(1).GetPlugins();
            Assert.That(viewModel.Plugins, Has.Count.EqualTo(2));
            Assert.That(viewModel.Plugins, Has.Member(plugin1));
            Assert.That(viewModel.Plugins, Has.Member(plugin2));
        }

        [Test]
        public void Ctor_PluginLoaderIsNull_PluginsIsEmpty()
        {
            // Arrange
            // Act
            var viewModel = new MainViewModel(null);

            // Assert
            Assert.That(viewModel.Plugins, Has.Count.EqualTo(0));
        }

        [Test]
        public void Ctor_PluginLoaderReturnNull_PluginsIsEmpty()
        {
            // Arrange
            var pluginLoader = Substitute.For<IPluginLoader>();
            pluginLoader.GetPlugins().Returns((IEnumerable)null);

            // Act
            var viewModel = new MainViewModel(pluginLoader);

            // Assert
            Assert.That(viewModel.Plugins, Has.Count.EqualTo(0));
        }

        [Test]
        public void Caption_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();
            var pluginLoader = Substitute.For<IPluginLoader>();
            var caption = "test";

            var viewModel = new MainViewModel(pluginLoader);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.Caption = caption;

            // Assert
            Assert.That(viewModel.Caption, Is.EqualTo(caption));

            eventHandler.Received(1).Invoke(viewModel, 
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.Caption)));
        }

        [Test]
        public void ActivePlugin_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();
            var pluginLoader = Substitute.For<IPluginLoader>();

            var plugin = Substitute.For<Plugin>();
            plugin.Name = "test";

            var viewModel = new MainViewModel(pluginLoader);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.ActivePlugin = plugin;

            // Assert
            Assert.That(viewModel.ActivePlugin, Is.EqualTo(plugin));

            eventHandler.Received(1).Invoke(viewModel,
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.ActivePlugin)));

            eventHandler.Received(1).Invoke(viewModel, 
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.VisiblePlugin)));
        }

        [Test]
        public void VisiblePlugin_ActivePluginIsNotNull_ReturnTrue()
        {
            // Arrange
            var pluginLoader = Substitute.For<IPluginLoader>();
            var plugin = Substitute.For<Plugin>();
            var viewModel = new MainViewModel(pluginLoader);

            // Act
            viewModel.ActivePlugin = plugin;

            // Assert
            Assert.That(viewModel.VisiblePlugin, Is.True);
        }

        [Test]
        public void VisiblePlugin_ActivePluginIsNull_ReturnFalse()
        {
            // Arrange
            var pluginLoader = Substitute.For<IPluginLoader>();
            var viewModel = new MainViewModel(pluginLoader);

            // Act
            viewModel.ActivePlugin = null;

            // Assert
            Assert.That(viewModel.VisiblePlugin, Is.False);
        }

        [Test, Apartment(ApartmentState.STA)]
        public void PluginOpenCommand_ExecCmd_PluginIsVisible()
        {
            // Arrange
            var page = Substitute.ForPartsOf<Page>();
            page.Visibility = Visibility.Hidden;

            var plugin = Substitute.For<Plugin>();
            plugin.Caption = "TestCaption";
            plugin.Name = "TestName";
            plugin.Page = page;

            var pluginLoader = Substitute.For<IPluginLoader>();
            var viewModel = new MainViewModel(pluginLoader);

            // Act
            viewModel.PluginOpenCommand.Execute(plugin);

            // Assert
            Assert.That(viewModel.ActivePlugin, Is.EqualTo(plugin));
            Assert.That(viewModel.Caption, Is.EqualTo(plugin.Caption));
            Assert.That(plugin.Page.Visibility, Is.EqualTo(Visibility.Visible));
        }

        [Test, Apartment(ApartmentState.STA)]
        public void PluginCloseCommand_ExecCmd_PluginIsNotVisible()
        {
            // Arrange
            var page = Substitute.ForPartsOf<Page>();
            page.Visibility = Visibility.Visible;

            var plugin = Substitute.For<Plugin>();
            plugin.Caption = "TestCaption";
            plugin.Name = "TestName";
            plugin.Page = page;

            var pluginLoader = Substitute.For<IPluginLoader>();

            var viewModel = new MainViewModel(pluginLoader)
            {
                ActivePlugin = plugin,
                Caption = plugin.Caption
            };

            // Act
            viewModel.PluginCloseCommand.Execute(default);

            // Assert
            Assert.That(viewModel.ActivePlugin, Is.Null);
            Assert.That(viewModel.Caption, Is.Not.EqualTo(plugin.Caption).And.Not.Empty);
            Assert.That(plugin.Page.Visibility, Is.EqualTo(Visibility.Hidden));
        }

    }
}
