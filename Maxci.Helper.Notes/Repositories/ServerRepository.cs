using Maxci.Helper.Notes.Entities;
using System.Collections.Generic;

namespace Maxci.Helper.Notes.Repositories
{
    class ServerRepository : IServerRepository
    {
        public string ServerURL { get; set; }

        public IEnumerable<dynamic> GetGroups()
        {
            return new[]
            {
                new NoteGroup
                {
                    Id = 1,
                    Name = "Group 1"
                },
                new NoteGroup
                {
                    Id = 2,
                    Name = "Group 2"
                },
            };
        }
    }
}
