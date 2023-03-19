using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Experience
{
    public class Expline
    {
        public int Id { get; set; }
        public short Level { get; set; }
        public long NecessaryExp { get; set; }
    }
}
