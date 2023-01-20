using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters.Races
{
    public class Race
    {
        public string Name { get; set; }
        public Attributes BaseAttributes { get; set; }
        public Characteristics BaseCharacteristics { get; set; }
        public List<Ability> RaceAbilities { get; set; }
    }
}
