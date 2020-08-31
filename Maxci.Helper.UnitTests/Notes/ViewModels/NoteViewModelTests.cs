using Maxci.Helper.Notes;
using Maxci.Helper.Notes.Entities;
using Maxci.Helper.Notes.Repositories;
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
            var mainViewModel = Substitute.For<MainViewModel>(db);

            var viewModel = new NoteViewModel(note, db, mainViewModel);
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
            var mainViewModel = Substitute.For<MainViewModel>(db);

            var viewModel = new NoteViewModel(note, db, mainViewModel);
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
            var mainViewModel = Substitute.For<MainViewModel>(db);

            var viewModel = new NoteViewModel(note, db, mainViewModel);
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
            var mainViewModel = Substitute.For<MainViewModel>(db);

            // Act
            var viewModel = new NoteViewModel(note, db, mainViewModel);

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
            var mainViewModel = Substitute.For<MainViewModel>(db);

            // Act
            void action()
            {
                var viewModel = new NoteViewModel(null, db, mainViewModel);
            };

            // Assert
            Assert.That(action, Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_DbIsNull_ThrowException()
        {
            // Arrange
            var note = new Note();
            var db = Substitute.For<IDbRepository>();
            var mainViewModel = Substitute.For<MainViewModel>(db);

            // Act
            void action()
            {
                var viewModel = new NoteViewModel(note, null, mainViewModel);
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
            var mainViewModel = Substitute.For<MainViewModel>(db);
            var viewModel = new NoteViewModel(note, db, mainViewModel)
            {
                NoteName = ""
            };

            // Act
            var result = viewModel.SaveCommand.CanExecute(default);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void SaveCommand_NewNoteAndNoteAddedInDB_NoteAddedAndSetCurrentNote()
        {
            // Arrange
            var noteName = "note_test";
            var idGroup = 234;
            var noteText = "234";
            var noteChanged = new DateTime(2001, 2, 3);
            var note = new Note { Id = 0, Name = noteName, IdGroup = idGroup, Text = noteText, Changed = noteChanged };

            var db = Substitute.For<IDbRepository>();
            db.AddNote(Arg.Any<Guid>(), idGroup, noteName, noteText).Returns(note);

            var mainViewModel = Substitute.For<MainViewModel>(db);
            var viewModel = new NoteViewModel(note, db, mainViewModel);

            // Act
            viewModel.SaveCommand.Execute(default);

            // Assert
            db.Received(1).AddNote(Arg.Any<Guid>(), idGroup, noteName, noteText);

            Assert.That(note.Changed, Is.Not.EqualTo(noteChanged));
            Assert.That(viewModel.NoteChanged, Is.Not.EqualTo(noteChanged));
            Assert.That(mainViewModel.Notes, Has.Member(note));
            Assert.That(mainViewModel.CurrentNote, Is.EqualTo(note));
        }

        [Test]
        public void SaveCommand_NewNoteAndDidntNoteAddInDB_DidntNoteAdd()
        {
            // Arrange
            var noteName = "note_test";
            var idGroup = 234;
            var noteText = "234";
            var noteChanged = new DateTime(2001, 2, 3);
            var note = new Note { Id = 0, Name = noteName, IdGroup = idGroup, Text = noteText, Changed = noteChanged };

            var db = Substitute.For<IDbRepository>();
            db.AddNote(Arg.Any<Guid>(), idGroup, noteName, noteText).Returns((Note)null);

            var mainViewModel = Substitute.For<MainViewModel>(db);
            var viewModel = new NoteViewModel(note, db, mainViewModel);
            
            // Act
            viewModel.SaveCommand.Execute(default);

            // Assert
            db.Received(1).AddNote(Arg.Any<Guid>(), idGroup, noteName, noteText);

            Assert.That(note.Changed, Is.EqualTo(noteChanged));
            Assert.That(mainViewModel.Notes, Has.No.Member(note));
        }

        [Test]
        public void SaveCommand_EditNoteAndNoteUpdatedInDB_CurrentNoteChanged()
        {
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

            var mainViewModel = Substitute.For<MainViewModel>(db);
            mainViewModel.Notes.Add(note);

            var viewModel = new NoteViewModel(note, db, mainViewModel);

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
        public void SaveCommand_EditNoteAndNoteDidntUpdateInDB_CurrentNoteDidntChange()
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

            var mainViewModel = Substitute.For<MainViewModel>(db);
            mainViewModel.Notes.Add(note);

            var viewModel = new NoteViewModel(note, db, mainViewModel);

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
