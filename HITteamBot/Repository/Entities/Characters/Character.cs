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
        public long Id { get; set; }
        public string Name { get; set; }
        public string Avatar { get; set; }
        public int Experience { get; set; }
        public short Level { get; set; }
        public long SPECIALsId { get; set; }
        public int Health { get; set; }
        public int CurrentHealth { get; set; }
        public int ActionPoints { get; set; }
        public int CurrentAP { get; set; }
        public short Rads { get; set; }
        public long EquipmentId { get; set; }
        public long Caps { get; set; }
        public bool IsAlive { get; set; }
    }
}
