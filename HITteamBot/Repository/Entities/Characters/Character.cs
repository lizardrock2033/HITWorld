using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Characters.Classes;
using HITteamBot.Repository.Entities.Characters.Races;
using HITteamBot.Repository.Entities.Items.Equipment;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Character
    {
        public string User { get; set; }
        public string Name { get; set; }
        public short Age { get; set; }
        public CharacterRaces? Race { get; set; }
        public string Story { get; set; }
        public CharacterClasses? Class { get; set; }
        public short Level { get; set; }
        public Characteristics Characteristics { get; set; }
        public Attributes Attributes { get; set; }
        public Skills Skills { get; set; }
        public List<Ability> Abilities { get; set; }
        public Equipment Equipment { get; set; }
        public Inventory Inventory { get; set; }
        public Locations.Locations CurrentLocation { get; set; }
        public LifeStates LifeState { get; set; }
        public Activities Activity { get; set; }
        public List<PositiveEffects> PositiveEffects { get; set; }
        public List<NegativeEffects> NegativeEffects { get; set; }
        public bool IsActive { get; set; }
    }
}
