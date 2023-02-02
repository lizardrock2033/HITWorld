using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Characteristics
    {
        public SPECIAL Attributes { get; set; }
        public int Health { get; set; }
        public int CurrentHealth { get; set; }
        public int ActionPoints { get; set; }
        public int CurrentAP { get; set; }
        public short WeightLimit { get;set;}
        public short CurrentWL { get;set; }
        public int Experience { get; set; }
        public int NextLevelOn { get; set; }
        public short Rads { get; set; }
        public RadContamination RadContamination { get; set; }
    }

    public enum RadContamination
    {
        Clear = 0,
        Light = 100,
        Normal = 300,
        High = 500,
        VeryHigh = 700,
        Lethal = 1000
    }
}
