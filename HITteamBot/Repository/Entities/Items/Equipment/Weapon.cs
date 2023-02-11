using System;
using System.Collections.Generic;
using System.Text;

namespace HITteamBot.Repository.Entities.Items.Equipment
{
    public class Weapon
    {
        public string Name { get; set; }
        public WeaponTypes Type { get; set; }
        public AmmoTypes AmmoType { get; set; }
        public int Damage { get; set; }
        public short FireRate { get; set; }
        public bool IsHeavy { get; set; }
    }

    public enum WeaponTypes
    {
        Cold,
        Firearm,
        Energy,
        Explosive
    }

    public enum AmmoTypes
    {
        None,
        Bullets,
        Battaries,
        Rockets
    }
}
