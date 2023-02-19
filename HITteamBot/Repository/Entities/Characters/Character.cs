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
        public short Level { get; set; }
        public long SPECIALsId { get; set; }
        public long CharacterPerksId { get; set; }
        public long CharacteristicsId { get; set; }
        public short LifeStateId { get; set; }
        public long EquipmentId { get; set; }
        public bool IsActive { get; set; }
    }
}
