using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Items.Equipment
{
    public class Equipment
    {
        public Weapon Weapon_1 { get; set; }
        public Weapon Weapon_2 { get; set; }
        public Armor Helmet { get; set; }
        public Armor Armor { get; set; }
        public Armor Gloves { get; set; }
        public Armor Boots { get; set; }
        public Clothes Clothes { get; set; }
    }
}
