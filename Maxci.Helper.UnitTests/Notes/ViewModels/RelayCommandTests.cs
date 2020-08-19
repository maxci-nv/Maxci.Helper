using Maxci.Helper.Notes.ViewModels;
using NSubstitute;
using NUnit.Framework;
using System;

namespace Maxci.Helper.UnitTests.Notes.ViewModels
{
    [TestFixture]
    class RelayCommandTests
    {
        [Test]
        public void Ctor_FirstArgumentIsNull_ThrowArgumentNullException()
        {
            // Arrange
            // Act
            static void action()
            {
                new RelayCommand(null, z => true);
            }

            // Assert
            Assert.That(action, Throws.ArgumentNullException);
        }

        [Test]
        public void CanExecute_CanExecuteIsNull_ReturnTrue()
        {
            // Arrange
            var cmd = new RelayCommand(z => { }, null);

            // Act
            var result = cmd.CanExecute(123);

            // Assert
            Assert.That(result, Is.True);
        }

        [Test]
        public void CanExecute_CanExecuteReturnTrue_ReturnTrue()
        {
            // Arrange
            var func = Substitute.For<Func<object, bool>>();
            func(default).ReturnsForAnyArgs(true);

            var cmd = new RelayCommand(z => { }, func);

            // Act
            var result = cmd.CanExecute(123);

            // Assert
            Assert.That(result, Is.True);
            func.Received(1).Invoke(123);
        }

        [Test]
        public void Execute_DelegateWithParameter_CallMethodWithParameter()
        {
            // Arrange
            var execute = Substitute.For<Action<object>>();
            var cmd = new RelayCommand(execute);
            var input = 234;

            // Act
            cmd.Execute(input);

            // Assert
            execute.Received(1).Invoke(input);
        }

    }
}
