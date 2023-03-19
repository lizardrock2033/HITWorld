using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace HITteamBot.Repository.Domain.DatabaseEntities.Characters
{
    public class SPECIAL
    {
        [Key]
        public long Id { get; set; }
        public short Strength { get; set; }
        public short Perception { get; set; }
        public short Endurance { get; set; }
        public short Charisma { get; set; }
        public short Intellegence { get; set; }
        public short Agility { get; set; }
        public short Luck { get; set; }
        public bool IsSet { get; set; }
    }

    public enum SPECIALs
    {
        Strength,
        Perception,
        Endurance,
        Charisma,
        Intellegence,
        Agility,
        Luck
    }
}
