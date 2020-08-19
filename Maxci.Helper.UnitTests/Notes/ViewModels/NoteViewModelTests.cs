using Maxci.Helper.Notes.Models;
using Maxci.Helper.Notes.ViewModels;
using NSubstitute;
using NSubstitute.ReceivedExtensions;
using NUnit.Framework;
using System;
using System.ComponentModel;

namespace Maxci.Helper.UnitTests.Notes.ViewModels
{
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0017:Упростите инициализацию объекта", Justification = "<Ожидание>")]
    [TestFixture]
    class NoteViewModelTests
    {
        [Test]
        public void NoteName_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var noteName = "string";
            var db = Substitute.For<IDbRepository>();
            var note = new Note();
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();

            var viewModel = new NoteViewModel(note, db);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.NoteName = noteName;

            // Assert
            Assert.That(viewModel.NoteName, Is.EqualTo(noteName));

            eventHandler.Received(1).Invoke(Arg.Any<object>(),
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.NoteName)));
        }

        [Test]
        public void NoteText_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var noteText = "string";
            var db = Substitute.For<IDbRepository>();
            var note = new Note();
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();

            var viewModel = new NoteViewModel(note, db);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.NoteText = noteText;

            // Assert
            Assert.That(viewModel.NoteText, Is.EqualTo(noteText));

            eventHandler.Received(1).Invoke(Arg.Any<object>(),
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.NoteText)));
        }

        [Test]
        public void NoteChanged_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var noteChanged = new DateTime(2001, 2, 3);
            var db = Substitute.For<IDbRepository>();
            var note = new Note();
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();

            var viewModel = new NoteViewModel(note, db);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.NoteChanged = noteChanged;

            // Assert
            Assert.That(viewModel.NoteChanged, Is.EqualTo(noteChanged));

            eventHandler.Received(1).Invoke(Arg.Any<object>(),
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.NoteChanged)));
        }

        [Test]
        public void Ctor_InitProperties_SetProperties()
        {
            // Arrange
            var noteName = "nameNote";
            var noteText = "textNote";
            var noteChanged = new DateTime(2001, 2, 3);
            var db = Substitute.For<IDbRepository>();
            var note = new Note
            {
                Id = 345,
                IdGroup = 234,
                Changed = noteChanged,
                Name = noteName,
                Text = noteText
            };

            // Act
            var viewModel = new NoteViewModel(note, db);

            // Assert
            Assert.That(viewModel.NoteName, Is.EqualTo(noteName));
            Assert.That(viewModel.NoteText, Is.EqualTo(noteText));
            Assert.That(viewModel.NoteChanged, Is.EqualTo(noteChanged));
        }

        [Test]
        public void Ctor_NoteIsNull_ThrowException()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();

            // Act
            void action()
            {
                var viewModel = new NoteViewModel(null, db);
            };

            // Assert
            Assert.That(action, Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_DbIsNull_ThrowException()
        {
            // Arrange
            var note = new Note();

            // Act
            void action()
            {
                var viewModel = new NoteViewModel(note, null);
            };

            // Assert
            Assert.That(action, Throws.ArgumentNullException);
        }

        [Test]
        public void SaveCommand_CantExecute_ReturnFalse()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var note = new Note { Id = 234, Name = "note_test" };
            var viewModel = new NoteViewModel(note, db)
            {
                NoteName = ""
            };

            // Act
            var result = viewModel.SaveCommand.CanExecute(default);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void SaveCommand_NoteUpdatedInDB_CurrentNoteChanged()
        {
            // Arrange
            var noteChangedOld = new DateTime(2001, 2, 3);
            var noteNameNew = "note_test_new";
            var noteTextNew = "note_text_new";
            var note = new Note
            { 
                Id = 234, 
                IdGroup = 345,
                Name = "note_test", 
                Text = "note_text", 
                Changed = noteChangedOld
            };

            var db = Substitute.For<IDbRepository>();
            db.UpdateNote(note.Id, note.IdGroup, noteNameNew, noteTextNew).Returns(true);

            var viewModel = new NoteViewModel(note, db);

            // Act
            viewModel.NoteName = noteNameNew;
            viewModel.NoteText = noteTextNew;

            viewModel.SaveCommand.Execute(default);

            // Assert
            db.Received(1).UpdateNote(note.Id, note.IdGroup, noteNameNew, noteTextNew);

            Assert.That(note.Name, Is.EqualTo(noteNameNew));
            Assert.That(note.Text, Is.EqualTo(noteTextNew));
            Assert.That(note.Changed, Is.Not.EqualTo(noteChangedOld));
        }

        [Test]
        public void SaveCommand_NoteDidntUpdateInDB_CurrentNoteDidntChange()
        {
            // Arrange
            var noteChangedOld = new DateTime(2001, 2, 3);
            var noteNameOld = "note_test";
            var noteTextOld = "note_text";
            var noteNameNew = "note_test_new";
            var noteTextNew = "note_text_new";
            var note = new Note
            {
                Id = 234,
                IdGroup = 345,
                Name = noteNameOld,
                Text = noteTextOld,
                Changed = noteChangedOld
            };

            var db = Substitute.For<IDbRepository>();
            db.UpdateNote(note.Id, note.IdGroup, noteNameNew, noteTextNew).Returns(false);

            var viewModel = new NoteViewModel(note, db);

            // Act
            viewModel.NoteName = noteNameNew;
            viewModel.NoteText = noteTextNew;

            viewModel.SaveCommand.Execute(default);

            // Assert
            db.Received(1).UpdateNote(note.Id, note.IdGroup, noteNameNew, noteTextNew);

            Assert.That(note.Name, Is.EqualTo(noteNameOld));
            Assert.That(note.Text, Is.EqualTo(noteTextOld));
            Assert.That(note.Changed, Is.EqualTo(noteChangedOld));
        }
    }
}
