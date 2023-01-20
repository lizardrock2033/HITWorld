using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Effect
    {
        public string Name { get; set; }
        public int Power { get; set; }
        public string Description { get; set; }
    }
    public enum PositiveEffects
    {
        None,
        Rested,
        Well_fed,
        Blessed
    }

    public enum NegativeEffects
    {
        None,
        Bleeding,
        Poisoned,
        Scared,
        Frozen,
        Overheated,
        Tired
    }
}
