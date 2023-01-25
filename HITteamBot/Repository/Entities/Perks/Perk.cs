using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Perks;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Perks
{
    public class Perk
    {
        public string Name { get; set; }
        public SPECIALs Attribute { get; set; }
        public short Cost { get; set; }
        public PerkTypes Type { get; set; }
        public Effect Action { get; set; }
        public string Description { get; set; }
    }

    public enum PerkTypes
    {
        Abylity,
        Bonus,
        Knowledge
    }
}
