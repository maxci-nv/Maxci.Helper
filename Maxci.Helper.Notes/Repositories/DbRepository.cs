using Dapper;
using Maxci.Helper.Notes.Entities;
using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SQLite;
using System.IO;
using System.Linq;
using System.Reflection;

namespace Maxci.Helper.Notes.Repositories
{
    class DbRepository : IDbRepository
    {
        private static readonly IDbConnection _db;

        static DbRepository()
        {
            var folderPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
            var relativePath = Path.GetRelativePath(Environment.CurrentDirectory, folderPlugin);

            File.Delete($"{relativePath}\\data.db");

            _db = new SQLiteConnection($"Data Source={relativePath}\\data.db;Version=3");

            SqlMapper.AddTypeHandler(new GuidTypeHandler());
            SqlMapper.AddTypeHandler(new DateTimeTypeHandler());
        }


        public DbRepository()
        {
            var folderPlugin = Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);

            if (!File.Exists($"{folderPlugin}\\data.db"))
                InitDB();
       }

        private void InitDB()
        {
            try
            {
                _db.Open();

                using var tran = _db.BeginTransaction();
                using var cmd = _db.CreateCommand();

                cmd.CommandText = "CREATE TABLE Groups ( " +
                                  "  Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                  "  Guid TEXT NOT NULL, " +
                                  "  Name TEXT NOT NULL, " +
                                  "  Del NUMERIC NOT NULL," +
                                  "  SyncId TEXT NOT NULL, " +
                                  "  SyncChanged INTEGER" +
                                  "); " +
                                  "" +
                                  "CREATE TABLE Notes ( " +
                                  "  Id INTEGER PRIMARY KEY AUTOINCREMENT, " +
                                  "  Guid TEXT NOT NULL, " +
                                  "  IdGroup INTEGER NOT NULL, " +
                                  "  Name TEXT NOT NULL, " +
                                  "  Text TEXT, " +
                                  "  Changed INTEGER NOT NULL, " +
                                  "  Del NUMERIC NOT NULL, " +
                                  "  SyncId TEXT NOT NULL, " +
                                  "  SyncChanged INTEGER NOT NULL, " +
                                  "  FOREIGN KEY(IdGroup) REFERENCES Groups(Id) " +
                                  ");" +
                                  "" +
                                  "CREATE UNIQUE INDEX UX_Groups_Guid ON Groups(Guid); " +
                                  "CREATE INDEX UX_Groups_Del ON Groups(Del); " +
                                  "CREATE INDEX UX_Notes_IdGroup ON Notes(IdGroup, Del); " +
                                  "CREATE UNIQUE INDEX UX_Notes_Guid ON Notes(Guid); ";

                cmd.ExecuteNonQuery();

                /*
                cmd.CommandText = "INSERT INTO Groups(Guid, Name, Del, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6581', 'group1', 0, 'cfe29e59-b87f-4386-83e2-e281791b6511', 1); " +
                                  "INSERT INTO Groups(Guid, Name, Del, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6582', 'group2', 0, 'cfe29e59-b87f-4386-83e2-e281791b6512', 1); " +
                                  "INSERT INTO Groups(Guid, Name, Del, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6583', 'group3', 0, 'cfe29e59-b87f-4386-83e2-e281791b6513', 1); " +
                                  
                                  "INSERT INTO Notes(Guid, IdGroup, Name, Text, Changed, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6571', 1, 'note1', '11', 637333729080000000, " +
                                  "  'cfe29e59-b87f-4386-83e2-e281791b6521', 1); " +
                                  "INSERT INTO Notes(Guid, IdGroup, Name, Text, Changed, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6572', 2, 'note2', '22', 637333729080000000, " +
                                  "  'cfe29e59-b87f-4386-83e2-e281791b6531', 1); " +
                                  "INSERT INTO Notes(Guid, IdGroup, Name, Text, Changed, SyncId, SyncChanged) " +
                                  "VALUES('cfe29e59-b87f-4386-83e2-e281791b6573', 1, 'note3', '33', 637333729080000000, " +
                                  "  'cfe29e59-b87f-4386-83e2-e281791b6541', 1); ";
                cmd.ExecuteNonQuery();
                */

                tran.Commit();
            }
            finally
            {
                _db.Close();
            }
        }

        public IEnumerable<NoteGroup> GetGroups()
        {
            try
            {
                _db.Open();

                return _db.Query<NoteGroup>("SELECT Id, Name FROM Groups WHERE Del=0 ORDER BY Name");
            }
            finally
            {
                _db.Close();
            }
        }

        public NoteGroup AddGroup(Guid guid, string name)
        {
            if (string.IsNullOrWhiteSpace(name))
                return null;

            name = name.Trim();

            try
            {
                _db.Open();

                return _db.Query<NoteGroup>("INSERT INTO Groups (Guid, Name, Del, SyncId, SyncChanged) " +
                                            "SELECT @guid, @name, 0, @sync, 1 " +
                                            "WHERE NOT EXISTS(SELECT 1 FROM Groups WHERE guid=@guid);" +
                                            "" +
                                            "SELECT Id, Name FROM Groups WHERE guid=@guid;", 
                                            new 
                                            {
                                                guid=guid.ToString(), 
                                                name, 
                                                sync = Guid.NewGuid().ToString()
                                            }).FirstOrDefault();
            }
            finally
            {
                _db.Close();
            }
        }

        public bool RemoveGroup(long id)
        {
            try
            {
                _db.Open();
                _db.Execute("UPDATE Groups " +
                            "SET Del=1, " +
                            "    SyncId=@sync," +
                            "    SyncChanged=1 " +
                            "WHERE id=@id; " +
                            "UPDATE Notes SET Del=1, SyncId=@sync, SyncChanged=1 WHERE IdGroup=@id AND Del=0"
                    , new { id, sync = Guid.NewGuid().ToString() });

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _db.Close();
            }
        }

        public bool RemoveGroupByGuid(Guid guid)
        {
            try
            {
                _db.Open();
                _db.Execute("UPDATE Groups " +
                            "SET Del=1, " +
                            "    SyncId=@sync, " +
                            "    SyncChanged=1 " +
                            "WHERE guid=@guid;", 
                    new 
                    { 
                        guid= guid.ToString(),
                        sync = Guid.NewGuid().ToString()
                    });

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _db.Close();
            }
        }

        public IEnumerable<Note> GetNotes(long idGroup)
        {
            try
            {
                _db.Open();

                return _db.Query<Note>("SELECT Id, IdGroup, Name, Text, Changed " +
                                       "FROM Notes " +
                                       "WHERE IdGroup=@idGroup AND Del=0 " +
                                       "ORDER BY Name", new { idGroup });
            }
            finally
            {
                _db.Close();
            }
        }

        public Note AddNote(Guid guid, long idGroup, string name, string text)
        {
            try
            {
                _db.Open();

                return _db.Query<Note>("INSERT INTO Notes (IdGroup, Guid, Name, Text, Changed, Del, SyncId, SyncChanged) " +
                                       "VALUES(@idGroup, @guid, @name, @text, @changed, 0, @sync, 1); " +
                                       "" +
                                       "SELECT ID, IdGroup, Name, Text, Changed " +
                                       "FROM Notes " +
                                       "WHERE guid=@guid"
                                       , new
                                       {
                                           idGroup,
                                           guid = guid.ToString(),
                                           name,
                                           text,
                                           changed = DateTime.UtcNow.Ticks,
                                           sync = Guid.NewGuid().ToString()
                                       }).FirstOrDefault();
            }
            finally
            {
                _db.Close();
            }
        }

        public bool RemoveNote(long id)
        {
            try
            {
                _db.Open();
                _db.Execute("UPDATE Notes " +
                            "SET Del=1, " +
                            "    SyncId=@sync, " +
                            "    SyncChanged=1 " +
                            "WHERE Id=@id"
                            , new 
                            { 
                                id,
                                sync = Guid.NewGuid().ToString()
                            });

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _db.Close();
            }
        }

        public bool UpdateNote(long id, long idGroup, string name, string text)
        {
            try
            {
                _db.Open();
                _db.Execute("UPDATE Notes " +
                            "SET IdGroup=@idGroup, " +
                            "    Name=@name, " +
                            "    Text=@text, " +
                            "    Changed=@changed, " +
                            "    SyncId=@sync, " +
                            "    SyncChanged=1 " +
                            "WHERE Id=@id", new 
                            { 
                                id, 
                                idGroup, 
                                name, 
                                text,
                                changed = DateTime.UtcNow,
                                sync = Guid.NewGuid().ToString()
                            });

                return true;
            }
            catch
            {
                return false;
            }
            finally
            {
                _db.Close();
            }
        }



        #region Data sync

        public IEnumerable<dynamic> GetGroupsForSync()
        {
            try
            {
                _db.Open();

                return _db.Query("SELECT Guid, SyncId, SyncChanged FROM Groups");
            }
            finally
            {
                _db.Close();
            }
        }

        public bool AddGroupForSync(Guid guid, string name, Guid sync)
        {
            throw new NotImplementedException();
        }

        public bool RemoveGroupForSync(Guid guid, Guid sync)
        {
            throw new NotImplementedException();
        }

        public bool AddNoteForSync(Guid guid, long idGroup, string name, string text, Guid sync)
        {
            throw new NotImplementedException();
        }

        public bool UpdateNoteForSync(Guid guid, long idGroup, string name, string text, Guid sync)
        {
            throw new NotImplementedException();
        }

        #endregion



        #region Private classes

        private class GuidTypeHandler : SqlMapper.TypeHandler<Guid>
        {
            public override void SetValue(IDbDataParameter parameter, Guid guid) => parameter.Value = guid.ToString();
            public override Guid Parse(object value) => new Guid((string)value);
        }

        private class DateTimeTypeHandler : SqlMapper.TypeHandler<DateTime>
        {
            public override void SetValue(IDbDataParameter parameter, DateTime dt) => parameter.Value = dt.Ticks.ToString();
            public override DateTime Parse(object value) => DateTime.SpecifyKind(new DateTime((long)value), DateTimeKind.Utc);
        }

        #endregion

    }
}
