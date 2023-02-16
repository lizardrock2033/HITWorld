using HITteamBot.Repository.Entities.Characters;
using HITteamBot.Repository.Entities.Locations;
using HITteamBot.Repository.Entities.Perks;
using HITteamBot.Repository.Entities.Items.Equipment;
using System;
using System.Collections.Generic;
using System.Text;
using Microsoft.EntityFrameworkCore;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Character
    {
        public string User { get; set; }
        public string Name { get; set; }
        public byte Age { get; set; }
        public string Gender { get; set; }
        public string Avatar { get; set; }
        public short Level { get; set; }
        public Characteristics Characteristics { get; set; }
        public List<Perk> Perks { get; set; }
        public Equipment Equipment { get; set; }
        public Inventory Inventory { get; set; }
        public LifeStates LifeState { get; set; }
        public Activities Activity { get; set; }
        public LocationTypes CurrentLocationType { get; set; }
        public List<CharacterStates> States { get; set; }
        public bool IsActive { get; set; }
    }
}
