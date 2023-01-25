using HITteamBot.Repository.Entities.Items.Ammo;
using HITteamBot.Repository.Entities.Items.Chemicals;
using HITteamBot.Repository.Entities.Items.Equipment;
using HITteamBot.Repository.Entities.Items.Junk;
using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Characters
{
    public class Inventory
    {
        public long Caps { get; set; }
        public Chemicals Chemicals { get; set; }
        public Ammo Ammo { get; set; }
        public Junk Junk { get; set; }
        public List<Weapon> Weapons { get; set; }
        public List<Armor> Armor { get; set; }
        public List<Clothes> Clothes { get; set; }
    }
}
