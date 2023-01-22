using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Characteristics
    {
        public SPECIAL Attributes { get; set; }
        public int Health { get; set; }
        public int ActionPoints { get; set; }
        public short WeightLimit { get;set;}
        public int Experience { get; set; }
        public int NextLevelOn { get; set; }
        public short Rads { get; set; }
    }
}
