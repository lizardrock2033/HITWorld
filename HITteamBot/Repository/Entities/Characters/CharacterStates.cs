using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class CharacterStates
    {
        public BaseStates State { get; set; }
        public short Duration { get; set; }
    }

    public enum BaseStates
    {
        None,
        Rested,
        Well_fed,
        Bleeding,
        Poisoned,
        Hungry,
        Frozen,
        Overheated,
        Tired,
        Depressed,
        Mad
    }
}
