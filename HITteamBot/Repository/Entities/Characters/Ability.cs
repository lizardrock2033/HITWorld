using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Ability
    {
        public string Name { get; set; }
        public byte LearningProgress { get; set; }
        public string Description { get; set; }
        public bool IsRaceAbility { get; set; }
    }
}
