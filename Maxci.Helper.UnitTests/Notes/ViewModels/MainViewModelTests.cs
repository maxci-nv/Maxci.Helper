using Maxci.Helper.Notes;
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
    [System.Diagnostics.CodeAnalysis.SuppressMessage("Style", "IDE0062:Сделать локальную функцию статической", Justification = "<Ожидание>")]
    [TestFixture]
    class MainViewModelTests
    {
        [Test]
        public void Ctor_DBRepositoryIsNull_ThrowException()
        {
            // Arrange
            // Act
            void action()
            {
                new MainViewModel(null);
            }

            // Assert
            Assert.That(action, Throws.ArgumentNullException);
        }

        [Test]
        public void Ctor_LoadNonEmptyListOfGroups_GroupsIsNotEmptyAndCurrentGroupIsNotNull()
        {
            // Arrange
            var group1 = new NoteGroup();
            var group2 = new NoteGroup();

            var db = Substitute.For<IDbRepository>();
            db.GetGroups().Returns(new[] { group1, group2 });

            // Act
            var viewModel = new MainViewModel(db);

            // Assert
            Assert.That(viewModel.NoteGroups, Has.Count.EqualTo(2));
            Assert.That(viewModel.NoteGroups, Has.Member(group1));
            Assert.That(viewModel.NoteGroups, Has.Member(group2));

            Assert.That(viewModel.CurrentGroup, Is.Not.Null);
            Assert.That(viewModel.CurrentGroup, Is.EqualTo(group1).Or.EqualTo(group2));
        }

        [Test]
        public void CurrentGroup_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var group = new NoteGroup();
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();

            var viewModel = new MainViewModel(db);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.CurrentGroup = group;

            // Assert
            Assert.That(viewModel.CurrentGroup, Is.EqualTo(group));

            eventHandler.Received(1).Invoke(Arg.Any<object>(),
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.CurrentGroup)));
        }

        [Test]
        public void CurrentGroup_SetValueAndGroupHasNotes_RefreshNotesAndCurrentNoteIsNull()
        {
            // Arrange
            var group = new NoteGroup { Id = 234 };
            var note1 = new Note { Id = 1};
            var note2 = new Note { Id = 2};
            var note3 = new Note { Id = 3 };

            var db = Substitute.For<IDbRepository>();
            db.GetNotes(group.Id).Returns(new[] { note1, note2 });

            var viewModel = new MainViewModel(db);
            viewModel.Notes.Add(note3);
            viewModel.CurrentNote = note3;

            // Act
            viewModel.CurrentGroup = group;

            // Assert
            Assert.That(viewModel.Notes, Has.Count.EqualTo(2));
            Assert.That(viewModel.Notes, Has.Member(note1).And.Member(note2));
            Assert.That(viewModel.CurrentNote, Is.Null);
        }

        [Test]
        public void CurrentNote_ChangeValue_EventOnPropertyChanged()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var note = new Note();
            var eventHandler = Substitute.For<PropertyChangedEventHandler>();

            var viewModel = new MainViewModel(db);
            viewModel.PropertyChanged += eventHandler;

            // Act
            viewModel.CurrentNote = note;

            // Assert
            Assert.That(viewModel.CurrentNote, Is.EqualTo(note));

            eventHandler.Received(1).Invoke(Arg.Any<object>(),
                Arg.Is<PropertyChangedEventArgs>(z => z.PropertyName == nameof(viewModel.CurrentNote)));
        }

        [Test]
        public void CurrentNote_SetValue_ShowNoteView()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var note = new Note();
            var viewModel = new MainViewModel(db);

            Plugin.WinManager = Substitute.For<IChildWindowManager>();

            // Act
            viewModel.CurrentNote = note;

            // Assert
            Plugin.WinManager.Received(1).ShowNoteView(note);
        }

        [TestCase(null)]
        [TestCase("")]
        [TestCase("  ")]
        public void AddGroupCommand_CantExecute_ReturnFalse(string param)
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var viewModel = new MainViewModel(db);

            // Act
            var result = viewModel.AddGroupCommand.CanExecute(param);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void AddGroupCommand_GroupAddedInDB_GroupAddedAndSetCurrentGroup()
        {
            // Arrange
            var group_old = new NoteGroup();
            var group_new = new NoteGroup
            {
                Id = 234,
                Name = "group_new",
            };

            var db = Substitute.For<IDbRepository>();
            db.AddGroup(Arg.Any<Guid>(), group_new.Name).Returns(group_new);

            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = group_old
            };

            // Act
            viewModel.AddGroupCommand.Execute(group_new.Name);

            // Assert
            db.Received(1).AddGroup(Arg.Any<Guid>(), group_new.Name);

            Assert.That(viewModel.NoteGroups, Has.Member(group_new));
            Assert.That(viewModel.CurrentGroup, Is.EqualTo(group_new));
        }

        [Test]
        public void AddGroupCommand_GroupDidntAddInDB_GroupDidntAddAndNotSetCurrentGroup()
        {
            // Arrange
            var groupName = "group_new";
            var group_old = new NoteGroup();

            var db = Substitute.For<IDbRepository>();
            db.AddGroup(Arg.Any<Guid>(), groupName).Returns((NoteGroup)null);

            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = group_old
            };

            // Act
            viewModel.AddGroupCommand.Execute(groupName);

            // Assert
            db.Received(1).AddGroup(Arg.Any<Guid>(), groupName);

            Assert.That(viewModel.CurrentGroup, Is.EqualTo(group_old));
            Assert.That(viewModel.NoteGroups, Is.Empty);
        }

        [Test]
        public void RemoveGroupCommand_CantExecute_ReturnFalse()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = null
            };

            // Act
            var result = viewModel.RemoveGroupCommand.CanExecute(default);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveGroupCommand_CurrentGroupInvalid_NotThrowExceptionAndDontCallRemoveGroupInDB()
        {
            // Arrange
            var group = new NoteGroup { Id = 234, Name = "group_text" };

            var db = Substitute.For<IDbRepository>();
            db.RemoveGroup(group.Id).Returns(true);

            var viewModel = new MainViewModel(db);
            viewModel.NoteGroups.Add(group);

            // Act
            viewModel.RemoveGroupCommand.Execute(default);

            // Assert
            db.DidNotReceiveWithAnyArgs().RemoveGroup(default);
        }

        [Test]
        public void RemoveGroupCommand_GroupRemovedInDB_GroupRemoved()
        {
            // Arrange
            var group = new NoteGroup { Id = 234, Name = "group_text" };

            var db = Substitute.For<IDbRepository>();
            db.RemoveGroup(group.Id).Returns(true);

            var viewModel = new MainViewModel(db);
            viewModel.NoteGroups.Add(group);
            viewModel.CurrentGroup = group;

            // Act
            viewModel.RemoveGroupCommand.Execute(default);

            // Assert
            Assert.That(viewModel.NoteGroups, Has.No.Member(group));
        }

        [Test]
        public void RemoveGroupCommand_GroupDidntRemoveInDB_GroupNotRemoved()
        {
            // Arrange
            var group = new NoteGroup { Id = 234, Name = "group_text" };

            var db = Substitute.For<IDbRepository>();
            db.RemoveGroup(group.Id).Returns(false);

            var viewModel = new MainViewModel(db);
            viewModel.NoteGroups.Add(group);
            viewModel.CurrentGroup = group;

            // Act
            viewModel.RemoveGroupCommand.Execute(default);

            // Assert
            Assert.That(viewModel.NoteGroups, Has.Member(group));
        }

        [Test]
        public void RemoveGroupCommand_GroupRemovedAndGroupsIsEmpty_CurrentGroupNull()
        {
            // Arrange
            var group = new NoteGroup { Id = 234, Name = "group_text" };

            var db = Substitute.For<IDbRepository>();
            db.RemoveGroup(group.Id).Returns(true);

            var viewModel = new MainViewModel(db);
            viewModel.NoteGroups.Add(group);
            viewModel.CurrentGroup = group;

            // Act
            viewModel.RemoveGroupCommand.Execute(default);

            // Assert
            Assert.That(viewModel.NoteGroups, Is.Empty);
            Assert.That(viewModel.CurrentGroup, Is.Null);
        }

        [Test]
        public void RemoveGroupCommand_GroupRemovedAndGroupsIsNotEmpty_CurrentGroupIsFirst()
        {
            // Arrange
            var group1 = new NoteGroup { Id = 2341, Name = "group_text1" };
            var group2 = new NoteGroup { Id = 2342, Name = "group_text2" };
            var group3 = new NoteGroup { Id = 2343, Name = "group_text3" };

            var db = Substitute.For<IDbRepository>();
            db.RemoveGroup(group3.Id).Returns(true);

            var viewModel = new MainViewModel(db);
            viewModel.NoteGroups.Add(group1);
            viewModel.NoteGroups.Add(group2);
            viewModel.NoteGroups.Add(group3);
            viewModel.CurrentGroup = group3;

            // Act
            viewModel.RemoveGroupCommand.Execute(default);

            // Assert
            Assert.That(viewModel.NoteGroups, Is.Not.Empty);
            Assert.That(viewModel.CurrentGroup, Is.EqualTo(viewModel.NoteGroups[0]));
        }

        [Test]
        public void AddNoteCommand_CantExecute_ReturnFalse()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = null
            };

            // Act
            var result = viewModel.AddNoteCommand.CanExecute(default);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void AddNoteCommand_CurrentGroupInvalid_DontThrowExceptionAndDontCallAddNoteInDB()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = null
            };

            // Act
            viewModel.AddNoteCommand.Execute(default);

            // Assert
            db.DidNotReceiveWithAnyArgs().AddNote(default, default, default, default);
        }

        [Test]
        public void AddNoteCommand_OpenNoteEditor_CurrentNoteIsNotNullAndNotesDontContainCurrentNote()
        {
            // Arrange
            var group = new NoteGroup { Id = 234 };
            var note = new Note { Id = 345 };

            var db = Substitute.For<IDbRepository>();
            db.AddNote(default, group.Id, default, default).ReturnsForAnyArgs((Note)null);

            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = group,
                CurrentNote = note
            };

            // Act
            viewModel.AddNoteCommand.Execute(default);

            // Assert
            Assert.That(viewModel.Notes, Is.Empty);
            Assert.That(viewModel.CurrentNote, Is.Not.Null);
            Assert.That(viewModel.CurrentNote.Id, Is.LessThanOrEqualTo(0));
            Assert.That(viewModel.CurrentNote.IdGroup, Is.EqualTo(group.Id));
        }

        [Test]
        public void RemoveNoteCommand_CantExecute_ReturnFalse()
        {
            // Arrange
            var db = Substitute.For<IDbRepository>();
            var viewModel = new MainViewModel(db)
            {
                CurrentGroup = null
            };

            // Act
            var result = viewModel.RemoveNoteCommand.CanExecute(default);

            // Assert
            Assert.That(result, Is.False);
        }

        [Test]
        public void RemoveNoteCommand_CurrentNoteIsNull_NoteDidntRemove()
        {
            // Arrange
            var note = new Note { Id = 234 };
            var db = Substitute.For<IDbRepository>();

            var viewModel = new MainViewModel(db);
            viewModel.Notes.Add(note);
            viewModel.CurrentNote = null;

            // Act
            viewModel.RemoveNoteCommand.Execute(default);

            // Assert
            Assert.That(viewModel.Notes, Has.Count.EqualTo(1));
            Assert.That(viewModel.Notes, Has.Member(note));

            db.DidNotReceiveWithAnyArgs().RemoveNote(default);
        }

        [Test]
        public void RemoveNoteCommand_NoteDidntRemoveInDB_NoteDidntRemove()
        {
            // Arrange
            var note = new Note { Id = 234 };

            var db = Substitute.For<IDbRepository>();
            db.RemoveNote(note.Id).Returns(false);

            var viewModel = new MainViewModel(db);
            viewModel.Notes.Add(note);
            viewModel.CurrentNote = note;

            // Act
            viewModel.RemoveNoteCommand.Execute(default);

            // Assert
            Assert.That(viewModel.Notes, Has.Count.EqualTo(1));
            Assert.That(viewModel.Notes, Has.Member(note));
        }

        [Test]
        public void RemoveNoteCommand_NoteRemovedInDB_NoteRemovedAndCurrentNoteIsNull()
        {
            // Arrange
            var note1 = new Note { Id = 2341 };
            var note2 = new Note { Id = 2342 };

            var db = Substitute.For<IDbRepository>();
            db.RemoveNote(note1.Id).Returns(true);

            var viewModel = new MainViewModel(db);
            viewModel.Notes.Add(note1);
            viewModel.Notes.Add(note2);
            viewModel.CurrentNote = note1;

            // Act
            viewModel.RemoveNoteCommand.Execute(default);

            // Assert
            Assert.That(viewModel.Notes, Has.Count.EqualTo(1));
            Assert.That(viewModel.Notes, Has.Member(note2));
            Assert.That(viewModel.CurrentNote, Is.Null);
        }
    }
}
